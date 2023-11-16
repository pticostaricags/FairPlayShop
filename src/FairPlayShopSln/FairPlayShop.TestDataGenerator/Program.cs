using FairPlayShop.DataAccess.Data;
using FairPlayShop.TestDataGenerator;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var connectionString =
            Environment.GetEnvironmentVariable("DefaultConnection") ??
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<FairPlayShopDatabaseContext>(optionsAction =>
{
    optionsAction.UseSqlServer(connectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(3),
            errorNumbersToAdd: null);
    });
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
