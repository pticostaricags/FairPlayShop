using FairPlayShop.DataAccess.Data;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Country;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class CountryService(FairPlayShopDatabaseContext fairPlayShopDatabaseContext) : ICountryService
    {
        public async Task<CountryModel[]?> GetCountryListAsync(CancellationToken cancellationToken)
        {
            var result = await fairPlayShopDatabaseContext.Country
                .AsNoTracking()
                .Select(p => new CountryModel()
                {
                    CountryId = p.CountryId,
                    Name = p.Name
                }).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
