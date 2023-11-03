using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
