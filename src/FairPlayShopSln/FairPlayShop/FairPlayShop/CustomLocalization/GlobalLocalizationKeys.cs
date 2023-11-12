using FairPlayShop.Common.CustomAttributes;
using Microsoft.Extensions.Localization;

namespace FairPlayShop.CustomLocalization
{
    public class GlobalKeysLocalizer
    {
        protected GlobalKeysLocalizer() { }
        public static IStringLocalizer<GlobalKeysLocalizer>? Localizer { get; set; }
        public static string SaveText => Localizer![SaveTextKey];
        public static string PaginatorSummaryText => Localizer![PaginatorSummaryTextKey];
        [ResourceKey(defaultValue: "Save")]
        public const string SaveTextKey = "SaveText";
        [ResourceKey("Page {0} of {1}. Total Items: {2}")]
        public const string PaginatorSummaryTextKey = "PaginatorSummaryText";
    }
}
