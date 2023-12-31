﻿using FairPlayShop.Models.StoreCustomer;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreCustomerService
    {
        Task CreateMyStoreCustomerAsync(CreateStoreCustomerModel createStoreCustomerModel, CancellationToken cancellationToken);
        Task<StoreCustomerModel> GetMyStoreCustomerAsync(long storeCustomerId, CancellationToken cancellationToken);
        Task<StoreCustomerModel[]?> GetMyStoreCustomerListAsync(long storeId, CancellationToken cancellationToken);
        Task DeleteMyStoreCustomerAsync(long storeCustomerId, CancellationToken cancellationToken);
    }
}
