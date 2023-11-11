using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StoreCustomerOrder;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class StoreCustomerOrderServiceTests : ServerSideServicesTestBase
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
            foreach (var singleCustomerOrderDetail in ctx.StoreCustomerOrderDetail)
            {
                ctx.StoreCustomerOrderDetail.Remove(singleCustomerOrderDetail);
            }
            foreach (var singleCustomerOrder in ctx.StoreCustomerOrder)
            {
                ctx.StoreCustomerOrder.Remove(singleCustomerOrder);
            }
            foreach (var singleProduct in ctx.Product)
            {
                ctx.Product.Remove(singleProduct);
            }
            foreach (var singleStoreCustomerAddress in ctx.StoreCustomerAddress)
            {
                ctx.Remove(singleStoreCustomerAddress);
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
            try
            {
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
        public async Task Test_CreateStoreCustomerOrderAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Product product = new()
            {
                AcquisitionCost = 12,
                Barcode = "Barcode",
                Description = "Description",
                Name = "Name",
                OwnerId = user.Id,
                ProductStatusId = (byte)Common.Enums.ProductStatus.Active,
                QuantityInStock = 10,
                SellingPrice = 20,
                Sku = "Sku",
                ThumbnailPhoto = new Photo()
                {
                    Name = nameof(Properties.Resources.TestProduct),
                    Filename = $"{Properties.Resources.TestProduct}.bmp",
                    PhotoBytes = Properties.Resources.TestProduct,
                }
            };
            Store store = new()
            {
                Name = $"AT Store: {nameof(Test_CreateStoreCustomerOrderAsync)}",
                OwnerId = user.Id,
                Product =
                {
                    product
                }
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            StoreCustomer storeCustomer = new()
            {
                EmailAddress = "t@t.t",
                Name = "AT Firstname",
                FirstSurname = "AT Lastname",
                StoreId = store.StoreId,
                PhoneNumber = "1234567890",
                SecondSurname = "AT Surname"
            };
            await ctx.StoreCustomer.AddAsync(storeCustomer);
            await ctx.SaveChangesAsync();
            CreateStoreCustomerOrderModel createStoreCustomerOrderModel = new()
            {
                StoreCustomerId = storeCustomer.StoreCustomerId,
                CreateStoreCustomerOrderDetailModel =
                [
                    new()
                    {
                        ProductId = product.ProductId,
                        Quantity = 1,
                        UnitPrice = product.SellingPrice
                    }
                ]
            };
            IStoreCustomerOrderService storeCustomerOrderService =
                await GetStoreCustomerOrderServiceAsync();
            await storeCustomerOrderService.CreateStoreCustomerOrderAsync(createStoreCustomerOrderModel,
                CancellationToken.None);
            var result = await ctx.StoreCustomerOrder.SingleOrDefaultAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(createStoreCustomerOrderModel.OrderTotal, result.OrderTotal);
        }

        [TestMethod]
        public async Task Test_GetStoreCustomerOrderListAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var user = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = user.Id;
            Product product = new()
            {
                AcquisitionCost = 12,
                Barcode = "Barcode",
                Description = "Description",
                Name = "Name",
                OwnerId = user.Id,
                ProductStatusId = (byte)Common.Enums.ProductStatus.Active,
                QuantityInStock = 1,
                SellingPrice = 20,
                Sku = "Sku",
                ThumbnailPhoto = new Photo()
                {
                    Name = nameof(Properties.Resources.TestProduct),
                    Filename = $"{Properties.Resources.TestProduct}.bmp",
                    PhotoBytes = Properties.Resources.TestProduct,
                }
            };
            Store store = new()
            {
                Name = $"AT Store 2",
                OwnerId = user.Id,
                Product =
                {
                    product
                }
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            StoreCustomer storeCustomer = new()
            {
                EmailAddress = "t@t.t",
                Name = "AT Firstname",
                FirstSurname = "AT Lastname",
                StoreId = store.StoreId,
                PhoneNumber = "1234567890",
                SecondSurname = "AT Surname"
            };
            await ctx.StoreCustomer.AddAsync(storeCustomer);
            await ctx.SaveChangesAsync();
            CreateStoreCustomerOrderModel createStoreCustomerOrderModel = new()
            {
                StoreCustomerId = storeCustomer.StoreCustomerId,
                CreateStoreCustomerOrderDetailModel =
                [
                    new Models.StoreCustomerOrderDetail.CreateStoreCustomerOrderDetailModel()
                    {
                        ProductId = product.ProductId,
                        Quantity = 1,
                        UnitPrice = product.SellingPrice
                    }
                ]
            };
            IStoreCustomerOrderService storeCustomerOrderService =
                await GetStoreCustomerOrderServiceAsync();
            await storeCustomerOrderService.CreateStoreCustomerOrderAsync(createStoreCustomerOrderModel,
                CancellationToken.None);
            var result = await storeCustomerOrderService.GetStoreCustomerOrderListAsync(
                store.StoreId, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(createStoreCustomerOrderModel.OrderTotal, result[0].OrderTotal);
        }
    }
}
