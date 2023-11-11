using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class StateOrProvinceServiceTests : ServerSideServicesTestBase
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
            foreach (var singleStateOrProvince in ctx.StateOrProvince)
            {
                ctx.StateOrProvince.Remove(singleStateOrProvince);
            }
            foreach (var singleCountry in ctx.Country)
            {
                ctx.Country.Remove(singleCountry);
            }
            await ctx.SaveChangesAsync();
        }
        [TestMethod]
        public async Task Test_GetCountryStateOrProvinceListAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            Country countryEntity = new()
            {
                Name = "Test Country",
            };
            StateOrProvince stateOrProvinceEntity = new()
            {
                Name = "Test State"
            };
            countryEntity.StateOrProvince.Add(stateOrProvinceEntity);
            await ctx.Country.AddAsync(countryEntity);
            await ctx.SaveChangesAsync();
            IStateOrProvinceService stateOrProvinceService = await GetStateOrProvinceServiceAsync();
            var result = await stateOrProvinceService
                .GetCountryStateOrProvinceListAsync(countryEntity.CountryId,
                CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(stateOrProvinceEntity.Name, result[0].Name);
        }
    }
}
