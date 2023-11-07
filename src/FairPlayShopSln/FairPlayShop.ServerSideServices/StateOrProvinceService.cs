using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StateOrProvince;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.ServerSideServices
{
    public class StateOrProvinceService(IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory)
        : IStateOrProvinceService
    {
        public async Task<StateOrProvinceModel[]?> GetCountryStateOrProvinceListAsync(
            long countryId, CancellationToken cancellationToken)
        {
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await fairPlayShopDatabaseContext.StateOrProvince
                .Where(p => p.CountryId == countryId)
                .Select(p => new StateOrProvinceModel()
                {
                    StateOrProvinceId = p.StateOrProvinceId,
                    Name = p.Name
                })
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
