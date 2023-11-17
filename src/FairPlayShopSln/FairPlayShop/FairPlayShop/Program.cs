using Azure.AI.OpenAI;
using Blazored.Toast;
using FairPlayShop.Client.Pages;
using FairPlayShop.Common;
using FairPlayShop.Common.CustomExceptions;
using FairPlayShop.Common.Enums;
using FairPlayShop.Components;
using FairPlayShop.Components.Account;
using FairPlayShop.CustomLocalization;
using FairPlayShop.CustomLocalization.EF;
using FairPlayShop.Data;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Product;
using FairPlayShop.ServerSideServices;
using FairPlayShop.Services;
using FairPlayShop.Translations;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Reflection;
namespace FairPlayShop;
internal static partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddTransient<IStringLocalizerFactory, EFStringLocalizerFactory>();
        builder.Services.AddTransient<IStringLocalizer, EFStringLocalizer>();
        builder.Services.AddLocalization();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
    .AddIdentityCookies();

        var connectionString =
            Environment.GetEnvironmentVariable("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddDbContextFactory<FairPlayShopDatabaseContext>(optionsAction =>
        {
            optionsAction.UseSqlServer(connectionString, sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null);
            });
        });

        var endpoint =
            Environment.GetEnvironmentVariable("AzureOpenAIEndpoint") ??
            throw new ConfigurationException("Can't find config for AzureOpenAI:Endpoint");
        var key = Environment.GetEnvironmentVariable("AzureOpenAIKey") ?? 
            throw new ConfigurationException("Can't find config for AzureOpenAI:Key");
        builder.Services.AddTransient<OpenAIClient>(sp =>
        {
            OpenAIClient openAIClient = new(endpoint: new Uri(endpoint),
                keyCredential: new Azure.AzureKeyCredential(key));
            return openAIClient;
        });
        builder.Services.AddTransient<IAzureOpenAIService, AzureOpenAIService>();
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddSingleton<IUserProviderService, UserProviderService>();
        builder.Services.AddTransient<IProductService, ProductService>();
        builder.Services.AddTransient<IStoreService, StoreService>();
        builder.Services.AddTransient<IStoreCustomerService, StoreCustomerService>();
        builder.Services.AddTransient<IStoreCustomerOrderService, StoreCustomerOrderService>();
        builder.Services.AddTransient<ICountryService, CountryService>();
        builder.Services.AddTransient<IStateOrProvinceService, StateOrProvinceService>();
        builder.Services.AddTransient<ICityService, CityService>();
        builder.Services.AddTransient<ICultureService, CultureService>();
        builder.Services.AddBlazoredToast();
        builder.Services.AddHostedService<BackgroundTranslationService>();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<FairPlayShopDatabaseContext>(name: "dbHealth");
        builder.Services.AddMemoryCache();
        var app = builder.Build();
        ConfigureModelsLocalizers(app.Services);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();
        using var scope = app.Services.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<FairPlayShopDatabaseContext>();
        var supportedCultures = ctx.Culture.Select(p => p.Name).ToArray();
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Counter).Assembly);


        app.MapControllers();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();
        if (app.Environment.IsDevelopment())
        {
            app.Use(async (context, next) =>
            {
                // Do work that can write to the Response.
                var roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                var role = await roleManager!.FindByNameAsync(Constants.RoleNames.SystemAdmin);
                if (role is null)
                {
                    role = new IdentityRole()
                    {
                        Name = Constants.RoleNames.SystemAdmin,
                        NormalizedName = Constants.RoleNames.SystemAdmin
                    };
                    await roleManager.CreateAsync(role);
                }
                var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var demoAdminUsername = "demoAdmin@test.test";
                var demoAdmin = await userManager.FindByNameAsync(demoAdminUsername);
                if (demoAdmin is null)
                {
                    demoAdmin = new ApplicationUser()
                    {
                        Email = demoAdminUsername,
                        EmailConfirmed = true,
                        UserName = demoAdminUsername
                    };
                    var result = await userManager.CreateAsync(demoAdmin, "Test12345!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(demoAdmin, role!.Name!);
                    }
                }
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });
        }
        app.MapHealthChecks("/appHealth");
        app.MapHealthChecks("/dbHealth");
        app.Run();
    }

    static void ConfigureModelsLocalizers(IServiceProvider services)
    {
        //Find a way to use Source Generators for this in order to avoid using reflection
        var modelsAssembly = typeof(FairPlayShop.Models.Store.CreateStoreModelLocalizer)
            .Assembly;
        var typesWithLocalizerAttribute =
            modelsAssembly.GetTypes().Where(p => p.CustomAttributes.Any(x => x.AttributeType.Name.Contains("LocalizerOfTAttribute")))
            .ToList();
        var localizerFactory = services.GetRequiredService<IStringLocalizerFactory>();
        foreach (var singleLocalizerType in typesWithLocalizerAttribute)
        {
            var newLocalizerInstance = localizerFactory.Create(singleLocalizerType);
            var field = singleLocalizerType.GetProperty("Localizer", BindingFlags.Public | BindingFlags.Static);
            field!.SetValue(null, newLocalizerInstance);
        }
        var globalKeysLocalizer =
        localizerFactory.Create(typeof(GlobalKeysLocalizer)) as IStringLocalizer<GlobalKeysLocalizer>;
        GlobalKeysLocalizer.Localizer = globalKeysLocalizer;
        var producStatusLocalizer =
        localizerFactory.Create(typeof(ProductStatusLocalizer)) as IStringLocalizer<ProductStatusLocalizer>;
        ProductStatusLocalizer.Localizer = producStatusLocalizer;

        var producModelLocalizer =
        localizerFactory.Create(typeof(ProductModelLocalizer)) as IStringLocalizer<ProductModelLocalizer>;
        ProductModelLocalizer.Localizer = producModelLocalizer;


    }
}