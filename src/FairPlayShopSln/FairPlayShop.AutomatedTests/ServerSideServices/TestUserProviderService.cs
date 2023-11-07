using FairPlayShop.Interfaces.Services;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    public class TestUserProviderService : IUserProviderService
    {
        public string GetCurrentUserId()
        {
            return ServerSideServicesTestBase.CurrentUserId!;
        }
    }
}
