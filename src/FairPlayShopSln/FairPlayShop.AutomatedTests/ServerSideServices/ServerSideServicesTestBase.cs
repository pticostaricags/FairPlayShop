using CsvHelper;
using CsvHelper.Configuration;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.ServerSideServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory)> GetFairPlayShopDatabaseContextAsync()
        {
            ServiceCollection services = new();
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
                var records = csvReader.GetRecords<TestResourcModel>().ToArray();
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
            return (fairPlayShopDatabaseContext, dbContextFactory);
        }

        private static TestUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }

        internal static async Task<IProductService> GetProductServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new ProductService(userProviderService, dbFactory);
        }

        internal static async Task<IStoreService> GetStoreServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbFactory) = await GetFairPlayShopDatabaseContextAsync();
            var loggerFactory = LoggerFactory.Create(p => p.AddConsole());
            var logger =
            loggerFactory.CreateLogger<StoreService>();
            return new StoreService(userProviderService, dbFactory, logger);
        }

        internal static async Task<IStoreCustomerService> GetStoreCustomerServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerService(userProviderService, dbFactory);
        }

        internal static async Task<ICountryService> GetCountryServiceAsync()
        {
            var (_, dbContextFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new CountryService(dbContextFactory);
        }

        internal static async Task<IStateOrProvinceService> GetStateOrProvinceServiceAsync()
        {
            var (_, dbContextFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new StateOrProvinceService(dbContextFactory);
        }

        internal static async Task<ICityService> GetCityServiceAsync()
        {
            var (_, dbContextFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new CityService(dbContextFactory);
        }

        internal static async Task<IStoreCustomerOrderService> GetStoreCustomerOrderServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (_, dbContextFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerOrderService(userProviderService, dbContextFactory);
        }
    }
}
