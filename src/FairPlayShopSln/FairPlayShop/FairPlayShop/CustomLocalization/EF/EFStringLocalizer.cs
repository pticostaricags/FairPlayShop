using FairPlayShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace FairPlayShop.CustomLocalization.EF
{
    /// <summary>
    /// Handles EF-based localization
    /// </summary>
    /// <remarks>
    /// Initializes <see cref="EFStringLocalizer"/>
    /// </remarks>
    /// <param name="dbContextFactory"></param>
    public class EFStringLocalizer(IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory) : IStringLocalizer
    {

        /// <summary>
        /// Returns the value for the given key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /// <summary>
        /// Returns the value for the given key using the supplied arguments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        /// <summary>
        /// Sets the Culture to use
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new EFStringLocalizer(dbContextFactory);
        }

        /// <summary>
        /// Gets all of the strings
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var db = dbContextFactory.CreateDbContext();
            return db.Resource
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string? GetString(string name)
        {
            var db = dbContextFactory.CreateDbContext();
            return db.Resource
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .FirstOrDefault(r => r.Key == name)?.Value;
        }
    }
}
