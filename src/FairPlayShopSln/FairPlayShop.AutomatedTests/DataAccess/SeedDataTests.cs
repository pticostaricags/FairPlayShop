using FairPlayShop.AutomatedTests.ServerSideServices;
using FairPlayShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.AutomatedTests.DataAccess
{
    [TestClass]
    public class SeedDataTests
    {
        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
        }

        [ClassCleanup()]
        public static async Task ClassCleanupAsync()
        {
            if (ServerSideServicesTestBase._msSqlContainer.State == DotNet.Testcontainers.Containers.TestcontainersStates.Running)
            {
                await ServerSideServicesTestBase._msSqlContainer.StopAsync();
            }
        }

        protected static async Task<FairPlayShopDatabaseContext> GetFairPlayShopDatabaseContextAsync()
        {
            DbContextOptionsBuilder<FairPlayShopDatabaseContext> dbContextOptionsBuilder =
                new();
            dbContextOptionsBuilder.UseSqlServer(ServerSideServicesTestBase._msSqlContainer.GetConnectionString(),
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

        [TestMethod]
        public async Task Test_SeedData_ExistsAsync()
        {
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            var productStatuses = await ctx.ProductStatus.ToArrayAsync();
            Assert.IsNotNull(productStatuses);
            Assert.AreEqual(2, productStatuses.Length);
            Assert.AreEqual(nameof(FairPlayShop.Common.Enums.ProductStatus.Draft), productStatuses[0].Name);
            Assert.AreEqual(nameof(FairPlayShop.Common.Enums.ProductStatus.Active), productStatuses[1].Name);
        }
    }
}
