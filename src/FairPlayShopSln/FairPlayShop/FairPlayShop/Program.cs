using Blazored.Toast;
using FairPlayShop.Client.Pages;
using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Components;
using FairPlayShop.CustomLocalization.EF;
using FairPlayShop.Data;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.Identity;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Store;
using FairPlayShop.Models.StoreCustomerOrder;
using FairPlayShop.ServerSideServices;
using FairPlayShop.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

internal partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTransient<IStringLocalizerFactory, EFStringLocalizerFactory>();
        builder.Services.AddTransient<IStringLocalizer, EFStringLocalizer>();
        builder.Services.AddLocalization();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<UserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
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

        builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
        builder.Services.AddSingleton<IUserProviderService, UserProviderService>();
        builder.Services.AddTransient<IProductService, ProductService>();
        builder.Services.AddTransient<IStoreService, StoreService>();
        builder.Services.AddTransient<IStoreCustomerService, StoreCustomerService>();
        builder.Services.AddTransient<IStoreCustomerOrderService, StoreCustomerOrderService>();
        builder.Services.AddTransient<ICountryService, CountryService>();
        builder.Services.AddTransient<IStateOrProvinceService, StateOrProvinceService>();
        builder.Services.AddTransient<ICityService, CityService>();
        builder.Services.AddBlazoredToast();
        var app = builder.Build();
        ConfigureModelsLocalizers(app.Services);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
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

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Counter).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.Run();
    }

    static void ConfigureModelsLocalizers(IServiceProvider services)
    {
        var localizerFactory = services.GetRequiredService<IStringLocalizerFactory>();
        CreateStoreModelLocalizer.Localizer =
            localizerFactory.Create(typeof(CreateStoreModelLocalizer))
            as IStringLocalizer<CreateStoreModelLocalizer>;
    }
    //[ConfigureModelsLocalizers]
    //static partial void ConfigureModelsLocalizers(IServiceProvider services);
}