using FairPlayShop.Models.Pagination;
using FairPlayShop.Models.Product;
using FairPlayShop.Models.Store;

namespace FairPlayShop.Interfaces.Services
{
    public interface IProductService
    {
        Task CreateMyProductAsync(CreateProductModel createProductModel, CancellationToken cancellationToken);
        Task<ProductModel[]> GetMyStoreProductListAsync(long storeId,CancellationToken cancellationToken);
        Task<PaginationOfT<ProductModel>> GetMyStoreProductListAsync(long storeId, int startIndex, CancellationToken cancellationToken);
        Task<ProductModel> GetMyProductByIdAsync(long productId, CancellationToken cancellationToken);
    }
}
