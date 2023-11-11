using FairPlayShop.Common.CustomAttributes;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace FairPlayShop.Common.Enums
{
    public enum ProductStatus
    {
        [Display(ResourceType = typeof(ProductStatusLocalizer),
            Name =nameof(ProductStatusLocalizer.Draft))]
        Draft = 1,
        [Display(ResourceType = typeof(ProductStatusLocalizer),
            Name = nameof(ProductStatusLocalizer.Active))]
        Active = 2
    }

    public class ProductStatusLocalizer
    {
        protected ProductStatusLocalizer() { }
        public static IStringLocalizer<ProductStatusLocalizer>? Localizer { get; set; }
        public static string Draft => Localizer![DraftTextKey];
        public static string Active => Localizer![ActiveTextKey];
        [ResourceKey(defaultValue: "Draft")]
        public const string DraftTextKey = "DraftText";
        [ResourceKey(defaultValue: "Active")]
        public const string ActiveTextKey = "ActiveText";
    }


}
