using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Common.Enums;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace FairPlayShop.Models.Product
{
    public class ProductModel
    {
        public long ProductId { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public string? Sku { get; set; }
        public int QuantityInStock { get; set; }
        public decimal AcquisitionCost { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        [Display(ResourceType =typeof(ProductModelLocalizer),
            Name = nameof(ProductModelLocalizer.ProductText))]
        public string? Name { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Profit { get; set; }
    }

    public class ProductModelLocalizer
    {
        public static IStringLocalizer<ProductModelLocalizer>? Localizer { get; set; }
        protected ProductModelLocalizer() { }
        public static string ProductText => Localizer![ProductTextKey];

        [ResourceKey(defaultValue: "Product")]
        public const string ProductTextKey = "ProductText";
    }
}
