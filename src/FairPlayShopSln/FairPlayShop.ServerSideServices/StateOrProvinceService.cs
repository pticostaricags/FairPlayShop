using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StateOrProvince;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class StateOrProvinceService(FairPlayShopDatabaseContext fairPlayShopDatabaseContext) 
        : IStateOrProvinceService
    {
        public async Task<StateOrProvinceModel[]?> GetCountryStateOrProvinceListAsync(
            long countryId, CancellationToken cancellationToken)
        {
            var result = await fairPlayShopDatabaseContext.StateOrProvince
                .Where(p => p.CountryId == countryId)
                .Select(p => new StateOrProvinceModel() 
                {
                    StateOrProvinceId = p.StateOrProvinceId,
                    Name = p.Name
                })
                .AsNoTracking()
                .OrderBy(p=>p.Name)
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
