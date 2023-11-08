using FairPlayShop.AutomatedTests.ServerSideServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.AutomatedTests.E2E
{
    [TestClass]
    public class RunAppTests : ServerSideServices.ServerSideServicesTestBase
    {
        private static CancellationTokenSource _cancellationTokenSource = new();
        private static WebApplication? AppInstance;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddUserSecrets<RunAppTests>();
            var config = configurationBuilder.Build();
            foreach (var singleItem in config.AsEnumerable())
            {
                Environment.SetEnvironmentVariable(singleItem.Key, singleItem.Value);
            }
            Environment.SetEnvironmentVariable("DefaultConnection", _msSqlContainer.GetConnectionString());
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            MainCaller(null, new string[] { "persistInstance",
            Properties.Resources.SeedData
            });
            AppInstance = AppInstanceGetter(null);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            AppInstance!.RunAsync(token: _cancellationTokenSource.Token);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        [ClassCleanup()]
        public static async Task ClassCleanupAsync()
        {
            if (ServerSideServicesTestBase._msSqlContainer.State == DotNet.Testcontainers.Containers.TestcontainersStates.Running)
            {
                await ServerSideServicesTestBase._msSqlContainer.StopAsync();
            }
            _cancellationTokenSource.Cancel();
        }

        [TestMethod]
        public async Task Test_RunAppAsync()
        {
            await Task.Delay(TimeSpan.FromMinutes(2));
            await Task.Yield();
        }

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "Main")]
        static extern void MainCaller(Program @this, string[] args);
        [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "AppInstance")]
        static extern ref WebApplication? AppInstanceGetter(Program @this);
    }
}
