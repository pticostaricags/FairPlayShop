using FairPlayShop.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Interfaces.Services
{
    public interface ICountryService
    {
        Task<CountryModel[]?> GetCountryListAsync(CancellationToken cancellationToken);
    }
}
