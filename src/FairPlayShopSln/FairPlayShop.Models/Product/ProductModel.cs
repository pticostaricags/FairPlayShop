using FairPlayShop.Common.Enums;

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
        public string? Name { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Profit { get; set; }
    }
}
