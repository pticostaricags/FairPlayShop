using FairPlayShop.Models.Pagination;
using FairPlayShop.Models.Store;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreService
    {
        Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken);
        Task<PaginationOfT<StoreModel>> GetPaginatedMyStoreListAsync(PaginationRequest paginationRequest, CancellationToken cancellationToken);
        Task<Uri[]> GetRecommendedLogoAsync(string[] storeProducts, CancellationToken cancellationToken);
        Task<string> GetStoreRecommendedNameAsync(string[] storeProducts, string[]? namesToExclude, CancellationToken cancellationToken);
    }
}
