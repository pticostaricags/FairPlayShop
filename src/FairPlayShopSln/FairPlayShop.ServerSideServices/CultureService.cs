using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class CultureService(FairPlayShopDatabaseContext fairPlayShopDatabaseContext) :
        ICultureService
    {
        public async Task<string[]> GetSupportedCultures(CancellationToken cancellationToken)
        {
            var result = await fairPlayShopDatabaseContext.Culture.AsNoTracking()
                .Select(c => c.Name)
                .ToArrayAsync(cancellationToken:cancellationToken);
            return result;
        }
    }
}
