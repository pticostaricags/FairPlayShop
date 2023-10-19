using FairPlayShop.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
