using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
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
            //TODO: Find a Translaton NuGet package that does not require external services
        }
    }
}
