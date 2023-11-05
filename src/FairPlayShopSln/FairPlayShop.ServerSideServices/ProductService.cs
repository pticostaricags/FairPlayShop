using FairPlayShop.Common;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Pagination;
using FairPlayShop.Models.Product;
using FairPlayShop.Models.Store;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Reflection.Metadata;

namespace FairPlayShop.ServerSideServices
{
    public class ProductService(IUserProviderService userProviderService,
        IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory) : IProductService
    {
        public async Task CreateMyProductAsync(CreateProductModel createProductModel, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            Product entity = new()
            {
                AcquisitionCost = createProductModel.AcquisitionCost!.Value,
                Barcode = createProductModel.Barcode,
                Description = createProductModel.Description,
                Name = createProductModel.Name,
                OwnerId = userId,
                ProductStatusId = (byte)createProductModel.ProductStatus!.Value,
                QuantityInStock = createProductModel.QuantityInStock!.Value,
                SellingPrice = createProductModel.SellingPrice!.Value,
                Sku = createProductModel.Sku,
                ThumbnailPhoto = new Photo()
                {
                    Name = createProductModel.PhotoName,
                    Filename = createProductModel.PhotoFilename,
                    PhotoBytes = createProductModel.PhotoBytes
                },
                StoreId = createProductModel.StoreId!.Value
            };
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            await fairPlayShopDatabaseContext.Product.AddAsync(entity, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<ProductModel> GetMyProductByIdAsync(long productId, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var result = await fairPlayShopDatabaseContext.Product.AsNoTracking()
                .Where(p => p.OwnerId == userId && p.ProductId == productId)
                .Select(p => new ProductModel()
                {
                    ProductId = p.ProductId,
                    ProductStatus = (Common.Enums.ProductStatus)p.ProductStatusId,
                    Sku = p.Sku,
                    QuantityInStock = p.QuantityInStock,
                    AcquisitionCost = p.AcquisitionCost,
                    Barcode = p.Barcode,
                    Description = p.Description,
                    Name = p.Name,
                    SellingPrice = p.SellingPrice,
                    Profit = p.SellingPrice - p.AcquisitionCost
                }).SingleAsync(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<ProductModel[]> GetMyStoreProductListAsync(long storeId, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var result = await fairPlayShopDatabaseContext.Product.AsNoTracking()
                .Where(p => p.OwnerId == userId && p.StoreId == storeId)
                .Select(p => new ProductModel()
                {
                    ProductId = p.ProductId,
                    ProductStatus = (Common.Enums.ProductStatus)p.ProductStatusId,
                    Sku = p.Sku,
                    QuantityInStock = p.QuantityInStock,
                    AcquisitionCost = p.AcquisitionCost,
                    Barcode = p.Barcode,
                    Description = p.Description,
                    Name = p.Name,
                    SellingPrice = p.SellingPrice,
                    Profit = p.SellingPrice - p.AcquisitionCost
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<PaginationOfT<ProductModel>> GetMyStoreProductListAsync(long storeId, int startIndex, CancellationToken cancellationToken)
        {
            PaginationOfT<ProductModel> result = new();
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var query = fairPlayShopDatabaseContext.Product.AsNoTracking()
                .Where(p => p.OwnerId == userId && p.StoreId == storeId);
            result.TotalItems = await query.CountAsync(cancellationToken: cancellationToken);
            result.PageSize = Constants.Pagination.PageSize;
            result.TotalPages = (int)Math.Ceiling((decimal)result.TotalItems / result.PageSize);
            result.Items = await query
                .Select(p => new ProductModel()
                {
                    ProductId = p.ProductId,
                    ProductStatus = (Common.Enums.ProductStatus)p.ProductStatusId,
                    Sku = p.Sku,
                    QuantityInStock = p.QuantityInStock,
                    AcquisitionCost = p.AcquisitionCost,
                    Barcode = p.Barcode,
                    Description = p.Description,
                    Name = p.Name,
                    SellingPrice = p.SellingPrice,
                    Profit = p.SellingPrice - p.AcquisitionCost
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
