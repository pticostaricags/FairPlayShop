using FairPlayShop.Models.Country;

namespace FairPlayShop.Interfaces.Services
{
    public interface ICountryService
    {
        Task<CountryModel[]?> GetCountryListAsync(CancellationToken cancellationToken);
    }
}
