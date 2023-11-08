using FairPlayShop.AutomatedTests.ServerSideServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.AutomatedTests.Playwright
{
    /// <summary>
    /// Based on: https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-1-224e8894c0f3
    /// </summary>
    [TestClass]
    public class PlaywrightTests : ServerSideServicesTestBase
    {
        private static IPlaywright? Playwright;
        private static Lazy<Task<IBrowser>>? ChromiumBrowser;
        private static Lazy<Task<IBrowser>>? FirefoxBrowser;
        private static Lazy<Task<IBrowser>>? WebkitBrowser;

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
            var exitCode = Microsoft.Playwright.Program.Main(
                new[] { "install-deps" });
            if (exitCode != 0)
            {
                throw new Exception(
                  $"Playwright exited with code {exitCode} on install-deps");
            }
            exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            if (exitCode != 0)
            {
                throw new Exception(
                  $"Playwright exited with code {exitCode} on install");
            }
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            // Setup Browser lazy initializers.
            ChromiumBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Chromium.LaunchAsync(options: new() 
              {
                  Headless = false,
              }));
            FirefoxBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Firefox.LaunchAsync(options:new()
              {
                  Headless=false
              }));
            WebkitBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Webkit.LaunchAsync(options:new()
              {
                  Headless=false
              }));
        }

        [ClassCleanup()]
        public static async Task ClassCleanupAsync()
        {
            if (ServerSideServicesTestBase._msSqlContainer.State == DotNet.Testcontainers.Containers.TestcontainersStates.Running)
            {
                await ServerSideServicesTestBase._msSqlContainer.StopAsync();
            }
            if (Playwright != null)
            {
                if (ChromiumBrowser != null && ChromiumBrowser.IsValueCreated)
                {
                    var browser = await ChromiumBrowser.Value;
                    await browser.DisposeAsync();
                }
                if (FirefoxBrowser != null && FirefoxBrowser.IsValueCreated)
                {
                    var browser = await FirefoxBrowser.Value;
                    await browser.DisposeAsync();
                }
                if (WebkitBrowser != null && WebkitBrowser.IsValueCreated)
                {
                    var browser = await WebkitBrowser.Value;
                    await browser.DisposeAsync();
                }
                Playwright.Dispose();
                Playwright = null;
            }
        }

        [TestMethod]
        public async Task Test_LoadHomeAsync()
        {
            var url = "https://localhost:5000";
            // Create the host factory with the App class as
            // parameter and the url we are going to use.
            using var hostFactory =
              new WebTestingHostFactory<FairPlayShop.Controllers.CultureController>();
            hostFactory
              // Override host configuration to mock stuff if required.
              .WithWebHostBuilder(builder =>
              {
                  // Setup the url to use.
                  builder.UseUrls(url);
                  // Replace or add services if needed.
                  builder.ConfigureServices(services =>
                  {
                      // services.AddTransient<....>();
                  })
          // Replace or add configuration if needed.
          .ConfigureAppConfiguration((app, conf) =>
                {
                    // conf.AddJsonFile("appsettings.Test.json");
                });
              })
              // Create the host using the CreateDefaultClient method.
              .CreateDefaultClient();
            // Open a page and run test logic.
            await GotoPageAsync(
              url,
              async (page) =>
              {
                  await page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();

                  await page.GetByRole(AriaRole.Main).ClickAsync();

                  await page.GetByRole(AriaRole.Heading, new() { Name = "Hello, world!" }).ClickAsync();
              },
              Browser.Chromium);
        }

        [TestMethod]
        public async Task Test_ChangeLanguageAsync()
        {
            var url = "https://localhost:5000";
            // Create the host factory with the App class as
            // parameter and the url we are going to use.
            using var hostFactory =
              new WebTestingHostFactory<FairPlayShop.Controllers.CultureController>();
            hostFactory
              // Override host configuration to mock stuff if required.
              .WithWebHostBuilder(builder =>
              {
                  // Setup the url to use.
                  builder.UseUrls(url);
                  // Replace or add services if needed.
                  builder.ConfigureServices(services =>
                  {
                      // services.AddTransient<....>();
                  })
          // Replace or add configuration if needed.
          .ConfigureAppConfiguration((app, conf) =>
          {
              // conf.AddJsonFile("appsettings.Test.json");
          });
              })
              // Create the host using the CreateDefaultClient method.
              .CreateDefaultClient();
            // Open a page and run test logic.
            await GotoPageAsync(
              url,
              async (page) =>
              {
                  await page.GetByRole(AriaRole.Main).ClickAsync();

                  await page.GetByRole(AriaRole.Combobox).SelectOptionAsync(new[] { "es-CR" });

                  await page.GetByRole(AriaRole.Link, new() { Name = "Inicio" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "Iniciar sesión" }).ClickAsync();
              },
              Browser.Chromium);
        }

        /// <summary>
        /// Browser types we can use in the PlaywrightFixture.
        /// </summary>
        public enum Browser
        {
            Chromium,
            Firefox,
            Webkit,
        }

        /// <summary>
        /// Open a Browser page and navigate to the given URL before
        /// applying the given test handler.
        /// </summary>
        /// <param name="url">URL to navigate to.</param>
        /// <param name="testHandler">Test handler to apply on the page.
        /// </param>
        /// <param name="browserType">The Browser to use to open the page.
        /// </param>
        /// <returns>The GotoPage task.</returns>
        public async Task GotoPageAsync(
            string url,
            Func<IPage, Task> testHandler,
            Browser browserType)
        {
            // select and launch the browser.
            var browser = await SelectBrowserAsync(browserType);
            // Create a new context with an option to ignore HTTPS errors.
            await using var context = await browser
              .NewContextAsync(
                new BrowserNewContextOptions
                {
                    IgnoreHTTPSErrors = true
                });
            // Open a new page.
            var page = await context.NewPageAsync();
            Assert.IsNotNull(page);
            try
            {
                // Navigate to the given URL and wait until loading
                // network activity is done.
                var gotoResult = await page.GotoAsync(
                  url,
                  new PageGotoOptions
                  {
                      WaitUntil = WaitUntilState.NetworkIdle
                  });
                Assert.IsNotNull(gotoResult);
                await gotoResult.FinishedAsync();
                Assert.IsTrue(gotoResult.Ok);
                // Run the actual test logic.
                await testHandler(page);
            }
            finally
            {
                // Make sure the page is closed 
                await page.CloseAsync();
            }
        }
        /// <summary>
        /// Select the IBrowser instance depending on the given browser
        /// enumeration value.
        /// </summary>
        /// <param name="browser">The browser to select.</param>
        /// <returns>The selected IBrowser instance.</returns>
        private Task<IBrowser> SelectBrowserAsync(Browser browser)
        {
            return browser switch
            {
                Browser.Chromium => ChromiumBrowser!.Value,
                Browser.Firefox => FirefoxBrowser!.Value,
                Browser.Webkit => WebkitBrowser!.Value,
                _ => throw new NotImplementedException(),
            };
        }

    }

    public class WebTestingHostFactory<TProgram>
  : WebApplicationFactory<TProgram>
  where TProgram : class
    {
        // Override the CreateHost to build our HTTP host server.
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Create the host that is actually used by the
            // TestServer (In Memory).
            var testHost = base.CreateHost(builder);
            // configure and start the actual host using Kestrel.
            builder.ConfigureWebHost(
              webHostBuilder => webHostBuilder.UseKestrel());
            var host = builder.Build();
            host.Start();
            // In order to cleanup and properly dispose HTTP server
            // resources we return a composite host object that is
            // actually just a way to intercept the StopAsync and Dispose
            // call and relay to our HTTP host.
            return new CompositeHost(testHost, host);
        }
    }

    // Relay the call to both test host and kestrel host.
    public class CompositeHost : IHost
    {
        private readonly IHost testHost;
        private readonly IHost kestrelHost;
        public CompositeHost(IHost testHost, IHost kestrelHost)
        {
            this.testHost = testHost;
            this.kestrelHost = kestrelHost;
        }
        public IServiceProvider Services => this.testHost.Services;
        public void Dispose()
        {
            this.testHost.Dispose();
            this.kestrelHost.Dispose();
        }
        public async Task StartAsync(
          CancellationToken cancellationToken = default)
        {
            await this.testHost.StartAsync(cancellationToken);
            await this.kestrelHost.StartAsync(cancellationToken);
        }
        public async Task StopAsync(
          CancellationToken cancellationToken = default)
        {
            await this.testHost.StopAsync(cancellationToken);
            await this.kestrelHost.StopAsync(cancellationToken);
        }
    }
}
