using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.ServerSideServices
{
    public class ProductService(IUserProviderService userProviderService,
        FairPlayShopDatabaseContext fairPlayShopDatabaseContext) : IProductService
    {
        public async Task CreateMyProductAsync(CreateProductModel createProductModel, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            Product entity = new Product()
            {
                AcquisitionCost = createProductModel.AcquisitionCost!.Value,
                Barcode = createProductModel.Barcode,
                Description = createProductModel.Description,
                Name = createProductModel.Name,
                OwnerId = userId,
                ProductStatusId = (byte)createProductModel.ProductStatus!.Value,
                QuantityInStock = createProductModel.QuantityInStock!.Value,
                SellingPrice = createProductModel.SellingPrice!.Value,
                Sku = createProductModel.Sku
            };
            await fairPlayShopDatabaseContext.Product.AddAsync(entity, cancellationToken:cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<ProductModel[]> GetMyProductListAsync(CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            var result = await fairPlayShopDatabaseContext.Product
                .Where(p => p.OwnerId == userId)
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
