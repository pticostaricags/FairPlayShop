using FairPlayShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace FairPlayShop.CustomLocalization.EF
{
    /// <summary>
    /// Handles EF-based localization
    /// </summary>
    public class EFStringLocalizer : IStringLocalizer
    {
        IDbContextFactory<FairPlayShopDatabaseContext> _dbContextFactory;

        /// <summary>
        /// Initializes <see cref="EFStringLocalizer"/>
        /// </summary>
        /// <param name="dbContextFactory"></param>
        public EFStringLocalizer(IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

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
            var db = this._dbContextFactory.CreateDbContext();
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new EFStringLocalizer(this._dbContextFactory);
        }

        /// <summary>
        /// Gets all of the strings
        /// </summary>
        /// <param name="includeAncestorCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            var db = this._dbContextFactory.CreateDbContext();
            return db.Resource
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string? GetString(string name)
        {
            var db = this._dbContextFactory.CreateDbContext();
            return db.Resource
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .FirstOrDefault(r => r.Key == name)?.Value;
        }
    }
}
