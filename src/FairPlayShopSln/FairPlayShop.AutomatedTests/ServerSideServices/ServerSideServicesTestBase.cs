using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.ServerSideServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ServiceCollection services = new ServiceCollection();
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
            return (fairPlayShopDatabaseContext, dbContextFactory);
        }

        private static TestUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }

        internal static async Task<IProductService> GetProductServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (fairPlayShopDatabaseContext, dbFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new ProductService(userProviderService, dbFactory);
        }

        internal static async Task<IStoreService> GetStoreServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (fairPlayShopDatabaseContext, dbFactory) = await GetFairPlayShopDatabaseContextAsync();
            var loggerFactory = LoggerFactory.Create(p => p.AddConsole());
            var logger =
            loggerFactory.CreateLogger<StoreService>();
            return new StoreService(userProviderService, dbFactory, logger);
        }

        internal static async Task<IStoreCustomerService> GetStoreCustomerServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            var (fairPlayShopDatabaseContext, dbFactory) = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerService(userProviderService, dbFactory);
        }

        internal static async Task<ICountryService> GetCountryServiceAsync()
        {
            var config = await GetFairPlayShopDatabaseContextAsync();
            return new CountryService(config.dbContextFactory);
        }

        internal static async Task<IStateOrProvinceService> GetStateOrProvinceServiceAsync()
        {
            var config = await GetFairPlayShopDatabaseContextAsync();
            return new StateOrProvinceService(config.dbContextFactory);
        }

        internal static async Task<ICityService> GetCityServiceAsync()
        {
            var config = await GetFairPlayShopDatabaseContextAsync();
            return new CityService(config.dbContextFactory);
        }

        internal static async Task<IStoreCustomerOrderService> GetStoreCustomerOrderServiceAsync()
        {
            var config = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerOrderService(config.dbContextFactory);
        }
    }
}
