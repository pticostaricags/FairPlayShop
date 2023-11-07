using FairPlayShop.Models.Pagination;
using FairPlayShop.Models.Store;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreService
    {
        Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken);
        Task<PaginationOfT<StoreModel>> GetPaginatedMyStoreListAsync(int startIndex, CancellationToken cancellationToken);
    }
}
