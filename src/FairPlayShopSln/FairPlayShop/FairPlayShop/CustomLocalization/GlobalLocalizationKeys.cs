using FairPlayShop.Common.CustomAttributes;
using Microsoft.Extensions.Localization;

namespace FairPlayShop.CustomLocalization
{
    public class GlobalKeysLocalizer
    {
        protected GlobalKeysLocalizer() { }
        public static IStringLocalizer<GlobalKeysLocalizer>? Localizer { get; set; }
        public static string SaveText => Localizer![SaveTextKey];
        [ResourceKey(defaultValue: "Save")]
        public const string SaveTextKey = "SaveText";
    }
}
