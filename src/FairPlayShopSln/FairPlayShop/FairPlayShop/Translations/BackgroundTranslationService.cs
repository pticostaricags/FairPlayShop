using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.AzureOpenAI;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FairPlayShop.Translations
{
    public class BackgroundTranslationService : BackgroundService
    {
        private readonly IServiceScopeFactory ServiceScopeFactory;

        private ILogger<BackgroundTranslationService> Logger { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="logger"></param>
        public BackgroundTranslationService(IServiceScopeFactory serviceScopeFactory,
            ILogger<BackgroundTranslationService> logger)
        {
            this.ServiceScopeFactory = serviceScopeFactory;
            this.Logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Process(stoppingToken);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(exception: ex, message: ex.Message);
            }
        }

        private async Task Process(CancellationToken stoppingToken)
        {
            using var scope = this.ServiceScopeFactory.CreateScope();
            FairPlayShopDatabaseContext fairplayshopDatabaseContext =
                scope.ServiceProvider.GetRequiredService<FairPlayShopDatabaseContext>();
            var modelsAssembly = typeof(Models.City.CityModel).Assembly;
            var modelsTypes = modelsAssembly.GetTypes();

            var serverAssembly = typeof(Program).Assembly;
            var serverTypes = serverAssembly.GetTypes();
            List<Type> typesToCheck = new();
            typesToCheck.AddRange(modelsTypes);
            typesToCheck.AddRange(serverTypes);

            foreach (var singleTypeToCheck in typesToCheck)
            {
                string typeFullName = singleTypeToCheck!.FullName!;
                var fields = singleTypeToCheck.GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy
                    );
                foreach (var singleField in fields)
                {
                    var resourceKeyAttributes =
                        singleField.GetCustomAttributes<ResourceKeyAttribute>();
                    if (resourceKeyAttributes != null && resourceKeyAttributes.Any())
                    {
                        ResourceKeyAttribute keyAttribute = resourceKeyAttributes.Single();
                        var defaultValue = keyAttribute.DefaultValue;
                        string key = singleField.GetRawConstantValue()!.ToString()!;
                        var entity =
                            await fairplayshopDatabaseContext.Resource
                            .SingleOrDefaultAsync(p => p.CultureId == 1 &&
                            p.Key == key &&
                            p.Type == typeFullName, stoppingToken);
                        if (entity is null)
                        {
                            entity = new Resource()
                            {
                                CultureId = 1,
                                Key = key,
                                Type = typeFullName,
                                Value = keyAttribute.DefaultValue
                            };
                            await fairplayshopDatabaseContext.Resource.AddAsync(entity, stoppingToken);
                        }
                    }
                }
            }
            if (fairplayshopDatabaseContext.ChangeTracker.HasChanges())
                await fairplayshopDatabaseContext.SaveChangesAsync(stoppingToken);
            var allEnglishUSKeys =
                await fairplayshopDatabaseContext.Resource
                .Include(p => p.Culture)
                .Where(p => p.Culture.Name == "en-US")
                .ToListAsync(stoppingToken);
            var allNonDefaultCulture = await fairplayshopDatabaseContext.Culture
                .Where(p => p.Name != "en-US").ToArrayAsync(stoppingToken);
            IAzureOpenAIService azureOpenAIService =
                scope.ServiceProvider.GetRequiredService<IAzureOpenAIService>();
            try
            {
                foreach (var resource in allEnglishUSKeys)
                {
                    foreach (var singleCulture in await fairplayshopDatabaseContext.Culture.ToArrayAsync(cancellationToken: stoppingToken))
                    {
                        if (await fairplayshopDatabaseContext.Resource
                            .AnyAsync(p => p.CultureId == singleCulture.CultureId &&
                            p.Key == resource.Key && p.Type == resource.Type) == false)
                        {
                            TranslationResponse? translationResponse = await
                                azureOpenAIService!
                                .TranslateSimpleTextAsync(resource.Value,
                                "en-US", singleCulture.Name,
                                cancellationToken: stoppingToken);
                            if (translationResponse != null)
                            {
                                await fairplayshopDatabaseContext.Resource
                                    .AddAsync(new Resource()
                                    {
                                        CultureId = singleCulture.CultureId,
                                        Key = resource.Key,
                                        Type = resource.Type,
                                        Value = translationResponse.TranslatedText ?? resource.Value
                                    },
                                    cancellationToken: stoppingToken);
                                await fairplayshopDatabaseContext
                                    .SaveChangesAsync(cancellationToken: stoppingToken);
                            }

                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Logger.LogError(ex.Message);
            }
            //TODO: Find a Translaton NuGet package that does not require external services
        }
    }
}
