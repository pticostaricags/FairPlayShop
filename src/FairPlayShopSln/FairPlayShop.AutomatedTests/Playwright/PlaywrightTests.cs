using FairPlayShop.AutomatedTests.ServerSideServices;
using FairPlayShop.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

namespace FairPlayShop.AutomatedTests.Playwright
{
    /// <summary>
    /// Based on: https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-1-224e8894c0f3
    /// </summary>
    [TestClass]
    public class PlaywrightTests : ServerSideServicesTestBase
    {
        private const string TEST_USER_USERNAME = "test@test.test";
        private const string TEST_USER_PASSWORD = "Test12345!";
        private static IPlaywright? Playwright;
        private static Lazy<Task<IBrowser>>? ChromiumBrowser;
        private static Lazy<Task<IBrowser>>? FirefoxBrowser;
        private static Lazy<Task<IBrowser>>? WebkitBrowser;
        private static readonly string[] args = ["install-deps"];
        private static readonly string[] argsArray = ["install"];
        private static readonly string[] values = ["es-CR"];

        #region Initialization
        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ServerSideServicesTestBase._msSqlContainer.StartAsync();
            Environment.SetEnvironmentVariable("DefaultConnection", _msSqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("skipTranslations", "true");
            await ServerSideServicesTestBase.GetFairPlayShopDatabaseContextAsync();
            var exitCode = Microsoft.Playwright.Program.Main(
                args);
            if (exitCode != 0)
            {
                throw new Exception(
                  $"Playwright exited with code {exitCode} on install-deps");
            }
            exitCode = Microsoft.Playwright.Program.Main(argsArray);
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
              Playwright.Firefox.LaunchAsync(options: new()
              {
                  Headless = false
              }));
            WebkitBrowser = new Lazy<Task<IBrowser>>(
              Playwright.Webkit.LaunchAsync(options: new()
              {
                  Headless = false
              }));
        }
        #endregion Initialization

        #region Cleanup
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

        [TestCleanup]
        public async Task TestCleanupAsync()
        {
            var (dbContext, _) = await GetFairPlayShopDatabaseContextAsync();
            var ctx = dbContext;
            foreach (var singleProduct in ctx.Product)
            {
                ctx.Product.Remove(singleProduct);
            }
            foreach (var singleStore in ctx.Store)
            {
                ctx.Store.Remove(singleStore);
            }
            foreach (var singleUser in ctx.AspNetUsers)
            {
                ctx.AspNetUsers.Remove(singleUser);
            }
            await ctx.SaveChangesAsync();
        }
        #endregion Cleanup

        #region Helpers
        private static WebTestingHostFactory<CultureController> CreateAppHost(out string url)
        {
            url = "https://localhost:5000";
            var tmpUrl = url;
            WebTestingHostFactory<CultureController> hostFactory = new();
            hostFactory
                          // Override host configuration to mock stuff if required.
                          .WithWebHostBuilder(builder =>
                          {
                              // Setup the url to use.
                              builder.UseUrls(tmpUrl);
                              // Replace or add services if needed.
                              builder.ConfigureServices(services =>
                              {
                              })
                      // Replace or add configuration if needed.
                      .ConfigureAppConfiguration((app, conf) =>
                      {
                      });
                          })
                          // Create the host using the CreateDefaultClient method.
                          .CreateDefaultClient();
            return hostFactory;
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
        public static async Task GotoPageAsync(
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
        private static Task<IBrowser> SelectBrowserAsync(Browser browser)
        {
            return browser switch
            {
                Browser.Chromium => ChromiumBrowser!.Value,
                Browser.Firefox => FirefoxBrowser!.Value,
                Browser.Webkit => WebkitBrowser!.Value,
                _ => throw new NotImplementedException(),
            };
        }
        #endregion Helpers

        [TestMethod]
        public async Task Test_RegisterNewUserAsync()
        {
            using var hostFactory = CreateAppHost(out string url);
            // Open a page and run test logic.
            await GotoPageAsync(
              url,
              async (page) =>
              {
                  await page.GotoAsync(url);

                  await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

                  await page.GetByPlaceholder("name@example.com").ClickAsync();

                  await page.GetByPlaceholder("name@example.com").FillAsync("test@test.test");

                  await page.GetByLabel("Password", new() { Exact = true }).ClickAsync();

                  await page.GetByLabel("Password", new() { Exact = true }).FillAsync(TEST_USER_PASSWORD);

                  await page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");

                  await page.GetByLabel("Confirm Password").FillAsync("Test12345!");

                  await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();

                  await page.GetByPlaceholder("name@example.com").ClickAsync();

                  await page.GetByPlaceholder("name@example.com").FillAsync(TEST_USER_USERNAME);

                  await page.GetByPlaceholder("name@example.com").PressAsync("Tab");

                  await page.GetByPlaceholder("password").FillAsync("Test12345!");

                  await page.GetByPlaceholder("password").PressAsync("Tab");

                  await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

                  await page.GetByRole(AriaRole.Heading, new() { Name = "Hello, world!" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "test@test.test" }).ClickAsync();

              },
              Browser.Chromium);
        }

        [TestMethod]
        public async Task Test_CreateStoreAsync()
        {
            using var hostFactory = CreateAppHost(out string url);
            // Open a page and run test logic.
            await GotoPageAsync(
              url,
              async (page) =>
              {
                  await page.GotoAsync(url);

                  await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

                  await page.GetByPlaceholder("name@example.com").ClickAsync();

                  await page.GetByPlaceholder("name@example.com").FillAsync("");

                  await page.GetByPlaceholder("name@example.com").PressAsync("CapsLock");

                  await page.GetByPlaceholder("name@example.com").FillAsync(TEST_USER_USERNAME);

                  await page.GetByPlaceholder("name@example.com").PressAsync("Tab");

                  await page.GetByLabel("Password", new() { Exact = true }).FillAsync(TEST_USER_PASSWORD);

                  await page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");

                  await page.GetByLabel("Confirm Password").FillAsync(TEST_USER_PASSWORD);

                  await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();

                  await page.GetByPlaceholder("name@example.com").ClickAsync();

                  await page.GetByPlaceholder("name@example.com").FillAsync(TEST_USER_USERNAME);

                  await page.GetByPlaceholder("name@example.com").PressAsync("Tab");

                  await page.GetByPlaceholder("password").FillAsync(TEST_USER_PASSWORD);

                  await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

                  await page.GetByRole(AriaRole.Link, new() { Name = "Create My Store" }).ClickAsync();

                  await page.GetByRole(AriaRole.Heading, new() { Name = "Create My Store" }).ClickAsync();

                  await page.GetByRole(AriaRole.Textbox).ClickAsync();

                  await page.GetByRole(AriaRole.Textbox).FillAsync("Store 1");

                  await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

                  await page.GetByRole(AriaRole.Heading, new() { Name = "My Store List" }).ClickAsync();

                  await page.GetByRole(AriaRole.Cell, new() { Name = "Store 1" }).ClickAsync();

              },
              Browser.Chromium);

        }

        [TestMethod]
        public async Task Test_LoadHomeAsync()
        {
            using var hostFactory = CreateAppHost(out string url);
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
            using var hostFactory = CreateAppHost(out string url);
            // Open a page and run test logic.
            await GotoPageAsync(
              url,
              async (page) =>
              {
                  await page.GetByRole(AriaRole.Main).ClickAsync();

                  await page.GetByRole(AriaRole.Combobox).SelectOptionAsync(values);

              },
              Browser.Chromium);
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
    public class CompositeHost(IHost testHost, IHost kestrelHost) : IHost
    {
        public IServiceProvider Services => testHost.Services;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            if (disposing)
            {
                testHost.Dispose();
                kestrelHost.Dispose();
            }
        }

        public async Task StartAsync(
          CancellationToken cancellationToken = default)
        {
            await testHost.StartAsync(cancellationToken);
            await kestrelHost.StartAsync(cancellationToken);
        }
        public async Task StopAsync(
          CancellationToken cancellationToken = default)
        {
            await testHost.StopAsync(cancellationToken);
            await kestrelHost.StopAsync(cancellationToken);
        }
    }
}
