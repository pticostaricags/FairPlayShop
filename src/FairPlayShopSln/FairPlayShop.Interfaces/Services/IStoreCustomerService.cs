using FairPlayShop.Models.StoreCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreCustomerService
    {
        Task CreateMyStoreCustomerAsync(CreateStoreCustomerModel createStoreCustomerModel, CancellationToken cancellationToken);
        Task<StoreCustomerModel> GetMyStoreCustomerAsync(long storeCustomerId, CancellationToken cancellationToken);
        Task<StoreCustomerModel[]?> GetMyStoreCustomerListAsync(long storeId, CancellationToken cancellationToken);
    }
}
