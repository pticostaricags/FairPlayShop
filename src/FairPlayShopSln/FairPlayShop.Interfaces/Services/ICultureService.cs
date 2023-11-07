namespace FairPlayShop.Interfaces.Services
{
    public interface ICultureService
    {
        Task<string[]> GetSupportedCultures(CancellationToken cancellationToken);
    }
}
