using FairPlayShop.Models.StoreCustomerOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreCustomerOrderService
    {
        Task CreateStoreCustomerOrderAsync(CreateStoreCustomerOrderModel createStoreCustomerOrderModel,
            CancellationToken cancellationToken);
        Task<StoreCustomerOrderModel[]?> GetStoreCustomerOrderListAsync(long storeId, CancellationToken cancellationToken);
    }
}
