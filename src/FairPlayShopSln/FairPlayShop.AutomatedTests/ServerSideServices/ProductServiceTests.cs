﻿using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Product;
using Microsoft.EntityFrameworkCore;

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

        [TestCleanup]
        public async Task TestCleanupAsync()
        {
            var (dbContext, _, _) = await GetFairPlayShopDatabaseContextAsync();
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
        public async Task Test_CreateMyProductAsync()
        {
            var (dbContext, _, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            Store store = new()
            {
                Name = $"AT Store: {nameof(Test_CreateMyProductAsync)}",
                OwnerId = userEntity.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IProductService productService = await ServerSideServicesTestBase.GetProductServiceAsync();
            CreateProductModel createProductModel = new()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                Name = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                QuantityInStock = 34,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft,
                PhotoName = nameof(Properties.Resources.TestProduct),
                PhotoFilename = $"{Properties.Resources.TestProduct}.bmp",
                PhotoBytes = Properties.Resources.TestProduct,
                StoreId = store.StoreId
            };
            await productService.CreateMyProductAsync(createProductModel, CancellationToken.None);
            var result = await ctx.Product.Include(p => p.ThumbnailPhoto).SingleOrDefaultAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(userEntity.Id, result.OwnerId);
            Assert.AreEqual($"Automated Test {nameof(Test_CreateMyProductAsync)}", result.Name);
            Assert.IsNotNull(result.ThumbnailPhoto);
            Assert.AreEqual(Properties.Resources.TestProduct.Length, result.ThumbnailPhoto.PhotoBytes.Length);
        }

        [TestMethod]
        public async Task Test_GetAllMyStoreProductListAsync()
        {
            var (dbContext, _, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            Store store = new()
            {
                Name = $"AT Store: {nameof(Test_GetAllMyStoreProductListAsync)}",
                OwnerId = userEntity.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IProductService productService = await ServerSideServicesTestBase.GetProductServiceAsync();
            CreateProductModel createProductModel = new()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                Name = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                QuantityInStock = 34,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft,
                PhotoName = nameof(Properties.Resources.TestProduct),
                PhotoFilename = $"{Properties.Resources.TestProduct}.bmp",
                PhotoBytes = Properties.Resources.TestProduct,
                StoreId = store.StoreId
            };
            await productService.CreateMyProductAsync(createProductModel, CancellationToken.None);
            var result = await productService.GetAllMyStoreProductListAsync(store.StoreId, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod]
        public async Task Test_GetAllMyStoreProductListInStockAsync()
        {
            var (dbContext, _, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            Store store = new()
            {
                Name = $"AT Store 1",
                OwnerId = userEntity.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IProductService productService = await ServerSideServicesTestBase.GetProductServiceAsync();
            CreateProductModel outOfStockProduct = new()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = "AT No Stock",
                Name = "AT No Stock",
                QuantityInStock = 0,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft,
                PhotoName = nameof(Properties.Resources.TestProduct),
                PhotoFilename = $"{Properties.Resources.TestProduct}.bmp",
                PhotoBytes = Properties.Resources.TestProduct,
                StoreId = store.StoreId
            };
            await productService.CreateMyProductAsync(outOfStockProduct, CancellationToken.None);
            CreateProductModel inStockProduct = new()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = "AT In Stock",
                Name = "AT In Stock",
                QuantityInStock = 34,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft,
                PhotoName = nameof(Properties.Resources.TestProduct),
                PhotoFilename = $"{Properties.Resources.TestProduct}.bmp",
                PhotoBytes = Properties.Resources.TestProduct,
                StoreId = store.StoreId
            };
            await productService.CreateMyProductAsync(inStockProduct, CancellationToken.None);
            var result = await productService.GetAllMyStoreProductListInStockAsync(store.StoreId, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(inStockProduct.QuantityInStock, result[0].QuantityInStock);
        }

        [TestMethod]
        public async Task Test_GetPaginatedMyStoreProductListAsync()
        {
            var (dbContext, _, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            Store store = new()
            {
                Name = $"AT Store: {nameof(Test_CreateMyProductAsync)}",
                OwnerId = userEntity.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IProductService productService = await ServerSideServicesTestBase.GetProductServiceAsync();
            CreateProductModel createProductModel = new()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                Name = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                QuantityInStock = 34,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft,
                PhotoName = nameof(Properties.Resources.TestProduct),
                PhotoFilename = $"{Properties.Resources.TestProduct}.bmp",
                PhotoBytes = Properties.Resources.TestProduct,
                StoreId = store.StoreId
            };
            await productService.CreateMyProductAsync(createProductModel, CancellationToken.None);
            var result = await productService
                .GetPaginatedMyStoreProductListAsync(store.StoreId, 0, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalItems);
            Assert.AreEqual(createProductModel.Name, result.Items![0].Name);
        }

        [TestMethod]
        public async Task Test_GetMyProductByIdAsync()
        {
            var (dbContext, _, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            var userEntity = await CreateTestUserAsync(ctx);
            ServerSideServicesTestBase.CurrentUserId = userEntity.Id;
            Store store = new()
            {
                Name = $"AT Store: {nameof(Test_CreateMyProductAsync)}",
                OwnerId = userEntity.Id,
            };
            await ctx.Store.AddAsync(store);
            await ctx.SaveChangesAsync();
            IProductService productService = await ServerSideServicesTestBase.GetProductServiceAsync();
            CreateProductModel createProductModel = new()
            {
                AcquisitionCost = 55.23M,
                Barcode = Guid.NewGuid().ToString(),
                Description = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                Name = $"Automated Test {nameof(Test_CreateMyProductAsync)}",
                QuantityInStock = 34,
                SellingPrice = 40,
                Sku = Guid.NewGuid().ToString(),
                ProductStatus = Common.Enums.ProductStatus.Draft,
                PhotoName = nameof(Properties.Resources.TestProduct),
                PhotoFilename = $"{Properties.Resources.TestProduct}.bmp",
                PhotoBytes = Properties.Resources.TestProduct,
                StoreId = store.StoreId
            };
            await productService.CreateMyProductAsync(createProductModel, CancellationToken.None);
            var entity = await ctx.Product.SingleAsync();
            var result = await productService.GetMyProductByIdAsync(entity.ProductId, CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(entity.Name, result.Name);
        }
    }
}
