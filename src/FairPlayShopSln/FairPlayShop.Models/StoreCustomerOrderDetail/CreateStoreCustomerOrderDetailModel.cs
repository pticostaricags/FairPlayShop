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
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.UnitPrice_Required))]
        public decimal? UnitPrice { get; set; }
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.Quantity_Required))]
        [Range(1, 100, ErrorMessageResourceType = typeof(CreateStoreCustomerOrderDetailModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderDetailModelLocalizer.Quantity_Range))]
        public decimal? Quantity { get; set; }
        public decimal? LineTotal => UnitPrice.GetValueOrDefault() * Quantity.GetValueOrDefault(0);
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreCustomerOrderDetailModel>]
    public partial class CreateStoreCustomerOrderDetailModelLocalizer
    {

    }
}
