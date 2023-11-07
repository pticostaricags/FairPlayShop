using FairPlayShop.Models.City;

namespace FairPlayShop.Interfaces.Services
{
    public interface ICityService
    {
        Task<CityModel[]?> GetStateOrProvinceCityListAsync(long stateOrProvinceId, CancellationToken cancellationToken);
    }
}
