using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Models.StoreCustomerOrderDetail;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FairPlayShop.Models.StoreCustomerOrder
{
    public class CreateStoreCustomerOrderModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderModelLocalizer.StoreCustomerId_Required))]
        public long? StoreCustomerId { get; set; }
        [Display(ResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            Name = nameof(CreateStoreCustomerOrderModelLocalizer.OrderSubTotalText))]
        public decimal OrderSubTotal => CreateStoreCustomerOrderDetailModel!.Sum(p => p.LineTotal.GetValueOrDefault(0));
        [Display(ResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            Name = nameof(CreateStoreCustomerOrderModelLocalizer.OrderTotalText))]
        public decimal OrderTotal => OrderSubTotal + TaxTotal;
        [Display(ResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            Name = nameof(CreateStoreCustomerOrderModelLocalizer.TaxTotalText))]
        public decimal TaxTotal => OrderSubTotal * ((decimal)13 / 100);
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderModelLocalizer.CreateStoreCustomerOrderDetailModel_Required))]
        [ValidateComplexType]
        [Length(minimumLength: 1, maximumLength: 50,
            ErrorMessageResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderModelLocalizer.CreateStoreCustomerOrderDetailModel_Length))]
        public List<CreateStoreCustomerOrderDetailModel>? CreateStoreCustomerOrderDetailModel { get; set; }
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreCustomerOrderModel>]
    public partial class CreateStoreCustomerOrderModelLocalizer
    {
        public static string OrderSubTotalText => Localizer![OrderSubTotalTextKey];
        public static string OrderTotalText => Localizer![OrderTotalTextKey];
        public static string TaxTotalText => Localizer![TaxTotalTextKey];
        [ResourceKey(defaultValue: "Order SubTotal")]
        public const string OrderSubTotalTextKey = "OrderSubTotalText";
        [ResourceKey(defaultValue: "Order Total")]
        public const string OrderTotalTextKey = "OrderTotalText";
        [ResourceKey(defaultValue: "Tax Total")]
        public const string TaxTotalTextKey = "TaxTotalText";
    }
}
