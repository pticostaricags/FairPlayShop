using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.ServerSideServices
{
    public class CultureService(IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory) :
        ICultureService
    {
        public async Task<string[]> GetSupportedCultures(CancellationToken cancellationToken)
        {
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await fairPlayShopDatabaseContext.Culture.AsNoTracking()
                .Select(c => c.Name)
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
