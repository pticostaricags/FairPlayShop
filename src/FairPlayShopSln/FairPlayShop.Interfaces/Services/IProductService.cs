using FairPlayShop.Models.Product;

namespace FairPlayShop.Interfaces.Services
{
    public interface IProductService
    {
        Task CreateMyProductAsync(CreateProductModel createProductModel, CancellationToken cancellationToken);
        Task<ProductModel[]> GetMyProductListAsync(CancellationToken cancellationToken);
    }
}
