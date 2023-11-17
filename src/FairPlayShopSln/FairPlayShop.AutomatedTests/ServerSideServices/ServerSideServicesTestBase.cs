using Azure.AI.OpenAI;
using CsvHelper;
using CsvHelper.Configuration;
using FairPlayShop.CustomLocalization.EF;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.ServerSideServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    public abstract class ServerSideServicesTestBase
    {
        public static string? CurrentUserId { get; protected set; }
        public static readonly MsSqlContainer _msSqlContainer
        = new MsSqlBuilder().Build();
        protected static async Task<(FairPlayShopDatabaseContext dbContext,
            IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory,
            ServiceProvider serviceProvider)> GetFairPlayShopDatabaseContextAsync()
        {
            ServiceCollection services = new();
            services.AddMemoryCache();
            services.AddDbContextFactory<FairPlayShopDatabaseContext>(options =>
            {
                options.UseSqlServer(_msSqlContainer.GetConnectionString(),
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(3),
                            errorNumbersToAdd: null);
                });
            });
            var sp = services.BuildServiceProvider();
            var dbContextFactory = sp.GetRequiredService<IDbContextFactory<FairPlayShopDatabaseContext>>();
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext =
                dbContextFactory.CreateDbContext();
            await fairPlayShopDatabaseContext.Database.EnsureCreatedAsync();
            await fairPlayShopDatabaseContext.Database.ExecuteSqlRawAsync(Properties.Resources.SeedData);
            await ImportTranslations(fairPlayShopDatabaseContext);
            return (fairPlayShopDatabaseContext, dbContextFactory, sp);
        }

        private static async Task ImportTranslations(FairPlayShopDatabaseContext fairPlayShopDatabaseContext)
        {
            if (!fairPlayShopDatabaseContext.Resource.Any())
            {
                using var reader = new StringReader(Properties.Resources.Translations);
                using CsvParser csvParser = new(reader, configuration:
                        new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                        {
                            Delimiter = ",",
                            ShouldQuote = ((ShouldQuoteArgs args) => { return false; })
                        });
                using CsvReader csvReader = new(csvParser);
                var records = csvReader.GetRecords<ImportResourceModel>().ToArray();
                records.AsParallel().ForAll(p => p.ResourceId = 0);
                foreach (var singleRecord in records)
                {
                    await fairPlayShopDatabaseContext.Resource.AddAsync(new Resource()
                    {
                        CultureId = singleRecord.CultureId,
                        Key = singleRecord.Key,
                        Type = singleRecord.Type,
                        Value = singleRecord.Value
                    });
                }
                await fairPlayShopDatabaseContext.SaveChangesAsync();
            }
        }

        private static TestUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }

        internal static async Task<IProductService> GetProductServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbFactory, _) = await GetFairPlayShopDatabaseContextAsync();
            return new ProductService(userProviderService, dbFactory);
        }

        internal static async Task<IStoreService> GetStoreServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbFactory, serviceProvider) = await GetFairPlayShopDatabaseContextAsync();
            var loggerFactory = LoggerFactory.Create(p => p.AddConsole());
            var logger =
            loggerFactory.CreateLogger<StoreService>();
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            IStringLocalizer<StoreService> localizer =
                new EFStringLocalizer<StoreService>(dbFactory, memoryCache);
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.AddUserSecrets<AzureOpenAIServiceTests>();
            var config = configurationBuilder.Build();
            var endpoint = config["AzureOpenAI:Endpoint"] ?? throw new Exception("Can't find config for AzureOpenAI:Endpoint");
            var key = config["AzureOpenAI:Key"] ?? throw new Exception("Can't find config for AzureOpenAI:Key");
            OpenAIClient openAIClient = new(new Uri(endpoint),
                new Azure.AzureKeyCredential(key));
            return new StoreService(userProviderService, dbFactory, logger, localizer,
                openAIClient);
        }

        internal static async Task<IStoreCustomerService> GetStoreCustomerServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbFactory, _) = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerService(userProviderService, dbFactory);
        }

        internal static async Task<ICountryService> GetCountryServiceAsync()
        {
            var (_, dbContextFactory, _) = await GetFairPlayShopDatabaseContextAsync();
            return new CountryService(dbContextFactory);
        }

        internal static async Task<IStateOrProvinceService> GetStateOrProvinceServiceAsync()
        {
            var (_, dbContextFactory, _) = await GetFairPlayShopDatabaseContextAsync();
            return new StateOrProvinceService(dbContextFactory);
        }

        internal static async Task<ICityService> GetCityServiceAsync()
        {
            var (_, dbContextFactory, _) = await GetFairPlayShopDatabaseContextAsync();
            return new CityService(dbContextFactory);
        }

        internal static async Task<IStoreCustomerOrderService> GetStoreCustomerOrderServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbContextFactory, _) = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerOrderService(userProviderService, dbContextFactory);
        }
    }
}
