using FairPlayShop.Models.StoreCustomerOrder;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreCustomerOrderService
    {
        Task CreateStoreCustomerOrderAsync(CreateStoreCustomerOrderModel createStoreCustomerOrderModel,
            CancellationToken cancellationToken);
        Task<StoreCustomerOrderModel[]?> GetStoreCustomerOrderListAsync(long storeId, CancellationToken cancellationToken);
    }
}
