using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.City;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class CityService(FairPlayShopDatabaseContext fairPlayShopDatabaseContext)
        : ICityService
    {
        public async Task<CityModel[]?> GetStateOrProvinceCityListAsync(long stateOrProvinceId, CancellationToken cancellationToken)
        {
            var result = await fairPlayShopDatabaseContext!.City
                .Where(p => p.StateOrProvinceId == stateOrProvinceId)
                .Select(p=>new CityModel()
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
