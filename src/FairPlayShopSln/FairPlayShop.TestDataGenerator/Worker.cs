using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace FairPlayShop.TestDataGenerator;

public class Worker(ILogger<Worker> logger,
    IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IDbContextFactory<FairPlayShopDatabaseContext> _dbContextFactory = dbContextFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
        int i = 0;
        await dbContext.Store.Where(p => p.Name.StartsWith("CGStore-")).ExecuteDeleteAsync(stoppingToken);
        await dbContext.SaveChangesAsync(cancellationToken:stoppingToken);
        int ui = 0;
        foreach (var user in await dbContext.AspNetUsers.ToArrayAsync(cancellationToken: stoppingToken))
        {
            if (user is not null)
            {
                while (!stoppingToken.IsCancellationRequested && i < 10000000)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }
                    string storeName = $"CGStore-U{ui}-{i++}";
                    _logger.LogInformation("Creating Store: {store}", storeName);
                    await dbContext.Store.AddAsync(new Store()
                    {
                        OwnerId = user.Id,
                        Name = storeName
                    }, stoppingToken);
                    if (i % 1000 == 0)
                    {
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Saving stores up to: {count}", i);
                    }
                }
            }
            ui++;
        }
    }
}
