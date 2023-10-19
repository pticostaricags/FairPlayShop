using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class ProductServiceTests : ServerSideServicesTestBase
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
        public async Task Test_CreateMyProductAsync()
        {
            var ctx = await GetFairPlayShopDatabaseContextAsync();
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            IProductService productService = await ServerSideServicesTestBase.GetProductServiceAsync();
            CreateProductModel createProductModel = new CreateProductModel()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                Name = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                QuantityInStock = 34,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft
            };
            await productService.CreateMyProductAsync(createProductModel, CancellationToken.None);
            var result = await ctx.Product.SingleOrDefaultAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(userEntity.Id, result.OwnerId);
            Assert.AreEqual(result.Name, $"Automated Test {nameof(Test_CreateMyProductAsync)}");

        }
    }
}
