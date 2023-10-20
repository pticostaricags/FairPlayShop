using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StoreCustomer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class StoreCustomerServiceTests : ServerSideServicesTestBase
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
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            foreach (var singleProduct in ctx.Product)
            {
                ctx.Product.Remove(singleProduct);
            }
            foreach (var singleStoreCustomer in ctx.StoreCustomer)
            {
                ctx.StoreCustomer.Remove(singleStoreCustomer);
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
        public async Task Test_CreateMyStoreCustomerAsync()
        {
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Store store = new Store()
            {
                Name = $"AT Store: {nameof(Test_CreateMyStoreCustomerAsync)}",
                OwnerId = user.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IStoreCustomerService storeCustomerService = await GetStoreCustomerServiceAsync();
            CreateStoreCustomerModel createStoreCustomerModel = new CreateStoreCustomerModel()
            {
                EmailAddress = "t@t.t",
                Firstname = "AT Firstname",
                Lastname = "AT Lastname",
                StoreId = store.StoreId,
                PhoneNumber = "1234567890",
                Surname = "AT Surname"
            };
            await storeCustomerService.CreateMyStoreCustomerAsync(createStoreCustomerModel, CancellationToken.None);
            var result = await ctx.StoreCustomer.SingleAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(createStoreCustomerModel.EmailAddress, result.EmailAddress);
        }

        [TestMethod]
        public async Task Test_GetMyStoreCustomerAsync()
        {
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Store store = new Store()
            {
                Name = $"AT Store: {nameof(Test_CreateMyStoreCustomerAsync)}",
                OwnerId = user.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IStoreCustomerService storeCustomerService = await GetStoreCustomerServiceAsync();
            StoreCustomer storeCustomer = new StoreCustomer()
            {
                EmailAddress = "t@t.t",
                Firstname = "AT Firstname",
                Lastname = "AT Lastname",
                StoreId = store.StoreId,
                PhoneNumber = "1234567890",
                Surname = "AT Surname"
            };
            await ctx.StoreCustomer.AddAsync(storeCustomer);
            await ctx.SaveChangesAsync();
            var result = await storeCustomerService.GetMyStoreCustomerAsync(storeCustomer.StoreCustomerId, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(storeCustomer.StoreCustomerId, result.StoreCustomerId);
        }

        [TestMethod]
        public async Task Test_GetMyStoreCustomerListAsync()
        {
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Store store = new Store()
            {
                Name = $"AT Store: {nameof(Test_CreateMyStoreCustomerAsync)}",
                OwnerId = user.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IStoreCustomerService storeCustomerService = await GetStoreCustomerServiceAsync();
            StoreCustomer storeCustomer = new StoreCustomer()
            {
                EmailAddress = "t@t.t",
                Firstname = "AT Firstname",
                Lastname = "AT Lastname",
                StoreId = store.StoreId,
                PhoneNumber = "1234567890",
                Surname = "AT Surname"
            };
            await ctx.StoreCustomer.AddAsync(storeCustomer);
            await ctx.SaveChangesAsync();
            var result = await storeCustomerService.GetMyStoreCustomerListAsync(store.StoreId, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(storeCustomer.EmailAddress, result[0].EmailAddress);
        }

        [TestMethod]
        public async Task Test_DeleteMyStoreCustomerAsync()
        {
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Store store = new Store()
            {
                Name = $"AT Store: {nameof(Test_CreateMyStoreCustomerAsync)}",
                OwnerId = user.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IStoreCustomerService storeCustomerService = await GetStoreCustomerServiceAsync();
            StoreCustomer storeCustomer = new StoreCustomer()
            {
                EmailAddress = "t@t.t",
                Firstname = "AT Firstname",
                Lastname = "AT Lastname",
                StoreId = store.StoreId,
                PhoneNumber = "1234567890",
                Surname = "AT Surname"
            };
            await ctx.StoreCustomer.AddAsync(storeCustomer);
            await ctx.SaveChangesAsync();
            var result = await ctx.StoreCustomer.SingleOrDefaultAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(storeCustomer.EmailAddress, result.EmailAddress);
            await storeCustomerService.DeleteMyStoreCustomerAsync(storeCustomer.StoreCustomerId, CancellationToken.None);
            result = await ctx.StoreCustomer.SingleOrDefaultAsync();
            Assert.IsNull(result);
        }
    }
}
