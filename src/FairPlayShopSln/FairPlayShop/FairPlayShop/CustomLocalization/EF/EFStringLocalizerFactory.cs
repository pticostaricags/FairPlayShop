using FairPlayShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace FairPlayShop.CustomLocalization.EF
{
    /// <summary>
    /// Handles EF-based lcoalization
    /// </summary>
    public class EFStringLocalizerFactory : IStringLocalizerFactory
    {
        private IDbContextFactory<FairPlayShopDatabaseContext> _dbContextFactory;

        /// <summary>
        /// Initializes <see cref="EFStringLocalizerFactory"/>
        /// </summary>
        /// <param name="dbContextFactory"></param>
        public EFStringLocalizerFactory(IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Creates the localizer
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <returns></returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            var localizerType = typeof(EFStringLocalizer<>)
                .MakeGenericType(resourceSource);
            var instance = Activator.CreateInstance(localizerType, args: new object[] { _dbContextFactory }) as IStringLocalizer;
            return instance!;
        }

        /// <summary>
        /// Create the localizeer using the location
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return new EFStringLocalizer(this._dbContextFactory);
        }
    }
}
