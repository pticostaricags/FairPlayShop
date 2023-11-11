using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class CountryServiceTests : ServerSideServicesTestBase
    {
        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
        }

        [ClassCleanup()]
        public static async Task ClassCleanupAsync()
        {
            if (ServerSideServicesTestBase._msSqlContainer.State == DotNet.Testcontainers.Containers.TestcontainersStates.Running)
            {
                await ServerSideServicesTestBase._msSqlContainer.StopAsync();
            }
        }

        [TestCleanup]
        public async Task TestCleanupAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            foreach (var singleCountry in ctx.Country)
            {
                ctx.Country.Remove(singleCountry);
            }
            await ctx.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Test_GetCountryListAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            Country country = new()
            {
                Name = "AT Test Country"
            };
            await ctx.Country.AddAsync(country);
            await ctx.SaveChangesAsync();
            ICountryService countryService = await GetCountryServiceAsync();
            var result = await countryService.GetCountryListAsync(CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(country.Name, result[0].Name);
        }
    }
}
