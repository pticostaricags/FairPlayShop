using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.ServerSideServices;
using Microsoft.EntityFrameworkCore;
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
        protected static async Task<FairPlayShopDatabaseContext> GetFairPlayShopDatabaseContextAsync()
        {
            DbContextOptionsBuilder<FairPlayShopDatabaseContext> dbContextOptionsBuilder =
                new();
            dbContextOptionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString(),
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(3),
                            errorNumbersToAdd: null);
                });
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext =
                new(dbContextOptionsBuilder.Options);
            await fairPlayShopDatabaseContext.Database.EnsureCreatedAsync();
            await fairPlayShopDatabaseContext.Database.ExecuteSqlRawAsync(Properties.Resources.SeedData);
            return fairPlayShopDatabaseContext;
        }

        private static TestUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }

        internal static async Task<IProductService> GetProductServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext = await GetFairPlayShopDatabaseContextAsync();
            return new ProductService(userProviderService, fairPlayShopDatabaseContext);
        }

        internal static async Task<IStoreService> GetStoreServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext = await GetFairPlayShopDatabaseContextAsync();
            return new StoreService(userProviderService, fairPlayShopDatabaseContext);
        }

        internal static async Task<IStoreCustomerService> GetStoreCustomerServiceAsync()
        {
            IUserProviderService userProviderService = GetUserProviderService();
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext = await GetFairPlayShopDatabaseContextAsync();
            return new StoreCustomerService(userProviderService, fairPlayShopDatabaseContext);
        }

        internal static async Task<ICountryService> GetCountryServiceAsync()
        {
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext = await GetFairPlayShopDatabaseContextAsync();
            return new CountryService(fairPlayShopDatabaseContext);
        }

        internal static async Task<IStateOrProvinceService> GetStateOrProvinceServiceAsync()
        {
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext = await GetFairPlayShopDatabaseContextAsync();
            return new StateOrProvinceService(fairPlayShopDatabaseContext);
        }

        internal static async Task<ICityService> GetCityServiceAsync()
        {
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext = await GetFairPlayShopDatabaseContextAsync();
            return new CityService(fairPlayShopDatabaseContext);
        }
    }
}
