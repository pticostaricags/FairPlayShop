using FairPlayShop.Common;
using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Common.CustomExceptions;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Pagination;
using FairPlayShop.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace FairPlayShop.ServerSideServices
{
    public class StoreService(IUserProviderService userProviderService,
        IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory,
        ILogger<StoreService> logger,
        IStringLocalizer<StoreService> localizer) : IStoreService
    {
        public async Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Executing {{ {nameof(createStoreModel)} }}", createStoreModel);
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            if (await fairPlayShopDatabaseContext.Store.AnyAsync(p => p.Name == createStoreModel.Name))
            {
                string message =
                    String.Format(
                    localizer[StoreNameExistTextKey], createStoreModel.Name);
                throw new RuleException(message);
            }
            var userId = userProviderService.GetCurrentUserId();
            Store entity = new()
            {
                Name = createStoreModel.Name,
                OwnerId = userId,
            };
            await fairPlayShopDatabaseContext.Store.AddAsync(entity, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<PaginationOfT<StoreModel>> GetPaginatedMyStoreListAsync(
            PaginationRequest paginationRequest, CancellationToken cancellationToken)
        {
            PaginationOfT<StoreModel> result = new();
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            string orderByString = string.Empty;
            if (paginationRequest.SortingItems?.Length > 0)
                orderByString =
                    String.Join(",",
                    paginationRequest.SortingItems.Select(p => $"{p.PropertyName} {GetSortTypeString(p.SortType)}"));
            var query = fairPlayShopDatabaseContext.Store.AsNoTracking()
                .Where(p => p.OwnerId == userId)
                .Select(p => new StoreModel()
                {
                    StoreId = p.StoreId,
                    Name = p.Name,
                    CustomerCount = p.StoreCustomer.Count
                });
            if (!String.IsNullOrEmpty(orderByString))
                query = query.OrderBy(orderByString);
            result.TotalItems = await query.CountAsync(cancellationToken: cancellationToken);
            result.TotalPages = (int)Math.Ceiling((decimal)result.TotalItems /
                Constants.Pagination.PageSize);
            result.PageSize = Constants.Pagination.PageSize;
            result.Items = await query.Skip(paginationRequest.StartIndex).Take(Constants.Pagination.PageSize)
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        private static string GetSortTypeString(SortType sortType)
        {
            return sortType == SortType.Ascending ? "ASC" : "DESC";
        }

        [ResourceKey(defaultValue: "There is already a store named '{0}'. Plaese use another")]
        public const string StoreNameExistTextKey = "StoreNameExistText";
    }
}
