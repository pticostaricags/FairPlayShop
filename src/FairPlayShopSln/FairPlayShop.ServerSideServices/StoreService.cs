using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class StoreService(IUserProviderService userProviderService,
        IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory,
        ILogger<StoreService> logger) : IStoreService
    {
        public async Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Executing {{{nameof(createStoreModel)}}}", createStoreModel);
            var userId = userProviderService.GetCurrentUserId();
            Store entity = new()
            {
                Name = createStoreModel.Name,
                OwnerId = userId,
            };
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            await fairPlayShopDatabaseContext.Store.AddAsync(entity, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<StoreModel[]?> GetMyStoreListAsync(CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var result = await fairPlayShopDatabaseContext.Store
                .Where(p=>p.OwnerId == userId)
                .AsNoTracking()
                .Select(p => new StoreModel() 
                {
                    StoreId = p.StoreId,
                    Name = p.Name,
                    CustomerCount = p.StoreCustomer.Count
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
