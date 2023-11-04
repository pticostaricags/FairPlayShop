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
        IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory) : IStoreCustomerService
    {
        public async Task CreateMyStoreCustomerAsync(CreateStoreCustomerModel createStoreCustomerModel, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            StoreCustomer storeCustomer = new()
            {
                EmailAddress = createStoreCustomerModel.EmailAddress,
                Name = createStoreCustomerModel.Name,
                FirstSurname = createStoreCustomerModel.FirstSurname,
                PhoneNumber = createStoreCustomerModel.PhoneNumber,
                StoreId = createStoreCustomerModel.StoreId!.Value,
                SecondSurname = createStoreCustomerModel.SecondSurname,
                StoreCustomerAddress = [
                    new StoreCustomerAddress()
                    {
                        AddressLine1 = createStoreCustomerModel.CreateStoreCustomerAddressModel!.AddressLine1,
                        AddressLine2 = createStoreCustomerModel.CreateStoreCustomerAddressModel.AddressLine2,
                        CityId = createStoreCustomerModel.CreateStoreCustomerAddressModel.CityId!.Value,
                        Company = createStoreCustomerModel.CreateStoreCustomerAddressModel.Company,
                        Name = createStoreCustomerModel.CreateStoreCustomerAddressModel.Name,
                        FirstSurname = createStoreCustomerModel.CreateStoreCustomerAddressModel.FirstSurname,
                        SecondSurname = createStoreCustomerModel.CreateStoreCustomerAddressModel.SecondSurname,
                        PhoneNumber = createStoreCustomerModel.CreateStoreCustomerAddressModel.PhoneNumber,
                        PostalCode = createStoreCustomerModel.CreateStoreCustomerAddressModel.PostalCode
                    }
                    ]
            };
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            await fairPlayShopDatabaseContext.StoreCustomer.AddAsync(storeCustomer, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteMyStoreCustomerAsync(long storeCustomerId, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var entity = await fairPlayShopDatabaseContext
                .StoreCustomer
                .Where(p => p.StoreCustomerId == storeCustomerId && p.Store.OwnerId == userId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (entity is not null)
            {
                fairPlayShopDatabaseContext.StoreCustomer.Remove(entity);
                await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            }
        }

        public async Task<StoreCustomerModel> GetMyStoreCustomerAsync(long storeCustomerId, CancellationToken cancellationToken)
        {
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var result = await fairPlayShopDatabaseContext.StoreCustomer.AsNoTracking()
                .Where(p => p.StoreCustomerId == storeCustomerId)
                .Select(p => new StoreCustomerModel()
                {
                    EmailAddress = p.EmailAddress,
                    Name = p.Name,
                    FirstSurname = p.FirstSurname,
                    PhoneNumber = p.PhoneNumber,
                    StoreId = p.StoreId,
                    StoreCustomerId = p.StoreCustomerId,
                    SecondSurname = p.SecondSurname
                }).SingleAsync(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<StoreCustomerModel[]?> GetMyStoreCustomerListAsync(long storeId, CancellationToken cancellationToken)
        {
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var result = await fairPlayShopDatabaseContext.StoreCustomer.AsNoTracking()
                .Where(p => p.StoreId == storeId)
                .Select(p => new StoreCustomerModel()
                {
                    EmailAddress = p.EmailAddress,
                    Name = p.Name,
                    FirstSurname = p.FirstSurname,
                    PhoneNumber = p.PhoneNumber,
                    StoreId = p.StoreId,
                    StoreCustomerId = p.StoreCustomerId,
                    SecondSurname = p.SecondSurname
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
