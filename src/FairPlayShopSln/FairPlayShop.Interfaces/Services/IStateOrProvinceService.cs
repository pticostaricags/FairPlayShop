using FairPlayShop.Models.StateOrProvince;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStateOrProvinceService
    {
        Task<StateOrProvinceModel[]?> GetCountryStateOrProvinceListAsync(long countryId, CancellationToken cancellationToken);
    }
}
