using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.City;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.ServerSideServices
{
    public class CityService(IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory)
        : ICityService
    {
        public async Task<CityModel[]?> GetStateOrProvinceCityListAsync(long stateOrProvinceId, CancellationToken cancellationToken)
        {
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var result = await fairPlayShopDatabaseContext!.City
                .Where(p => p.StateOrProvinceId == stateOrProvinceId)
                .OrderBy(p => p.Name)
                .Select(p => new CityModel()
                {
                    CityId = p.CityId,
                    Name = p.Name
                })
                .AsNoTracking()
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
