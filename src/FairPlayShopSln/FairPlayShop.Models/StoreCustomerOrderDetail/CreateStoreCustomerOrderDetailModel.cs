using FairPlayShop.Common.CustomAttributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FairPlayShop.Models.StoreCustomerOrderDetail
{
    public class CreateStoreCustomerOrderDetailModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.ProductId_Required))]
        public long? ProductId { get; set; }
        [Display(ResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            Name = nameof(CreateStoreCustomerOrderDetailModelLocalizer.UnitPriceText))]
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.UnitPrice_Required))]
        public decimal? UnitPrice { get; set; }
        [Display(ResourceType =typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            Name = nameof(CreateStoreCustomerOrderDetailModelLocalizer.QuantityText))]
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.Quantity_Required))]
        [Range(1, 100, ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.Quantity_Range))]
        public decimal? Quantity { get; set; }
        [Display(ResourceType =typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            Name =nameof(CreateStoreCustomerOrderDetailModelLocalizer.LineTotalText))]
        public decimal? LineTotal => UnitPrice.GetValueOrDefault() * Quantity.GetValueOrDefault(0);
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreCustomerOrderDetailModel>]
    public partial class CreateStoreCustomerOrderDetailModelLocalizer
    {
        public static string UnitPriceText => Localizer![UnitPriceTextKey];
        public static string QuantityText => Localizer![QuantityTextKey];
        public static string LineTotalText => Localizer![LineTotalTextKey];
        [ResourceKey(defaultValue: "Unit Price")]
        public const string UnitPriceTextKey = "UnitPriceText";
        [ResourceKey(defaultValue: "Quantity")]
        public const string QuantityTextKey = "QuantityText";
        [ResourceKey(defaultValue: "Line Total")]
        public const string LineTotalTextKey = "LineTotalText";
    }
}
