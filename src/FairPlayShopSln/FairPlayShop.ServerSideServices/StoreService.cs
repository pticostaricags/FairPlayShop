using FairPlayShop.Common;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Pagination;
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

        public async Task<PaginationOfT<StoreModel>> GetPaginatedMyStoreListAsync(int startIndex, CancellationToken cancellationToken)
        {
            PaginationOfT<StoreModel> result = new PaginationOfT<StoreModel>();
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken:cancellationToken);
            var query = fairPlayShopDatabaseContext.Store
                .Where(p => p.OwnerId == userId)
                .AsNoTracking();
            result.TotalItems = await query.CountAsync(cancellationToken: cancellationToken);
            result.TotalPages = (int)Math.Ceiling((decimal)result.TotalItems / 
                Constants.Pagination.PageSize);
            result.PageSize = Constants.Pagination.PageSize;
            result.Items = await query.Skip(startIndex).Take(Constants.Pagination.PageSize)
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
