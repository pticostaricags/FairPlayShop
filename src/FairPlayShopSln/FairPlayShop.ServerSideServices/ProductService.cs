using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Product;

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
    }
}
