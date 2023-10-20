using FairPlayShop.Models.Product;

namespace FairPlayShop.Interfaces.Services
{
    public interface IProductService
    {
        Task CreateMyProductAsync(CreateProductModel createProductModel, CancellationToken cancellationToken);
        Task<ProductModel[]> GetMyStoreProductListAsync(long storeId,CancellationToken cancellationToken);
        Task<ProductModel> GetMyProductByIdAsync(long productId, CancellationToken cancellationToken);
    }
}
