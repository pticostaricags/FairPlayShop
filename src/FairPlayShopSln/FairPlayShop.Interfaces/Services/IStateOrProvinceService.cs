using FairPlayShop.Models.StateOrProvince;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStateOrProvinceService
    {
        Task<StateOrProvinceModel[]?> GetCountryStateOrProvinceListAsync(long countryId, CancellationToken cancellationToken);
    }
}
