using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StoreCustomer;
using Microsoft.EntityFrameworkCore;
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

        public async Task<StoreCustomerModel> GetMyStoreCustomerAsync(long storeCustomerId, CancellationToken cancellationToken)
        {
            var result = await fairPlayShopDatabaseContext.StoreCustomer.AsNoTracking()
                .Where(p=>p.StoreCustomerId == storeCustomerId)
                .Select(p=> new StoreCustomerModel()
                {
                    EmailAddress = p.EmailAddress,
                    Firstname= p.Firstname,
                    Lastname= p.Lastname,
                    PhoneNumber = p.PhoneNumber,
                    StoreId = p.StoreId,
                    StoreCustomerId = p.StoreCustomerId,
                    Surname = p.Surname
                }).SingleAsync(cancellationToken:cancellationToken);
            return result;
        }
    }
}
