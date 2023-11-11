using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Store;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class StoreServiceTests : ServerSideServicesTestBase
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

        [TestCleanup]
        public async Task TestCleanupAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            foreach (var singleProduct in ctx.Product)
            {
                ctx.Product.Remove(singleProduct);
            }
            foreach (var singleStore in ctx.Store)
            {
                ctx.Store.Remove(singleStore);
            }
            foreach (var singleUser in ctx.AspNetUsers)
            {
                ctx.AspNetUsers.Remove(singleUser);
            }
            await ctx.SaveChangesAsync();
        }

        private static async Task<AspNetUsers> CreateTestUserAsync(FairPlayShopDatabaseContext ctx)
        {
            AspNetUsers userEntity = new()
            {
                Id = Guid.NewGuid().ToString(),
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "test@test.test",
                EmailConfirmed = false,
                LockoutEnabled = false,
                NormalizedEmail = "test@test.test",
                NormalizedUserName = "test@test.test",
                PasswordHash = Guid.NewGuid().ToString(),
                PhoneNumber = "111-1111-1111",
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
                UserName = "test@test.test"
            };
            await ctx.AspNetUsers.AddAsync(userEntity);
            await ctx.SaveChangesAsync();
            return userEntity;
        }

        [TestMethod]
        public async Task Test_CreateMyStoreAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            IStoreService storeService = await GetStoreServiceAsync();
            CreateStoreModel createStoreModel = new()
            {
                Name = "Automated Test Store"
            };
            await storeService.CreateMyStoreAsync(createStoreModel, CancellationToken.None);
            var result = await ctx.Store.SingleAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(createStoreModel.Name, result.Name);
        }

        [TestMethod]
        public async Task Test_GetMyStoreListAsync()
        {
            var (dbContext, dbContextFactory) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Store store = new()
            {
                Name = $"AT Store {nameof(Test_GetMyStoreListAsync)}",
                OwnerId = user.Id
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IStoreService storeService = await GetStoreServiceAsync();
            var result = await storeService.GetPaginatedMyStoreListAsync(
                paginationRequest: new()
                {
                    SortingItems = null,
                    StartIndex = 0
                }
                , CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalItems);
            Assert.AreEqual(store.Name, result.Items![0].Name);
        }
    }
}
