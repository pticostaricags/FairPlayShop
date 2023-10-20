using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StoreCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class StoreCustomerService(IUserProviderService userProviderService,
        FairPlayShopDatabaseContext fairPlayShopDatabaseContext) : IStoreCustomerService
    {
        public async Task CreateMyStoreCustomerAsync(CreateStoreCustomerModel createStoreCustomerModel, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            StoreCustomer storeCustomer = new StoreCustomer()
            {
                EmailAddress = createStoreCustomerModel.EmailAddress,
                Firstname = createStoreCustomerModel.Firstname,
                Lastname = createStoreCustomerModel.Lastname,
                PhoneNumber = createStoreCustomerModel.PhoneNumber,
                StoreId = createStoreCustomerModel.StoreId!.Value,
                Surname = createStoreCustomerModel.Surname,
            };
            await fairPlayShopDatabaseContext.StoreCustomer.AddAsync(storeCustomer, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken:cancellationToken);
        }
    }
}
