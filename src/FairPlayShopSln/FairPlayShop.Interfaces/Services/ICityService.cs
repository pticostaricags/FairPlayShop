using FairPlayShop.Models.City;
using FairPlayShop.Models.StateOrProvince;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Interfaces.Services
{
    public interface ICityService
    {
        Task<CityModel[]?> GetStateOrProvinceCityListAsync(long stateOrProvinceId, CancellationToken cancellationToken);
    }
}
