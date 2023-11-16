using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataImporter;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
var connectionString = 
    Environment.GetEnvironmentVariable("DefaultConnection") ??
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<FairPlayShopDatabaseContext>(optionsAction =>
{
    optionsAction.UseSqlServer(connectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(3),
            errorNumbersToAdd: null);
    });
}, contextLifetime: ServiceLifetime.Singleton, optionsLifetime: ServiceLifetime.Singleton);
builder.Services.AddHostedService<CitiesImporter>();

var host = builder.Build();
host.Run();
