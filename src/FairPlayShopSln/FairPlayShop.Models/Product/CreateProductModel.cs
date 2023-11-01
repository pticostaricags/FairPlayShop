using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayShop.Common.Enums;
using FairPlayShop.Common.CustomAttributes;

namespace FairPlayShop.Models.Product
{
    public class CreateProductModel
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        public int? QuantityInStock { get; set; }

        [Required]
        public decimal? SellingPrice { get; set; }

        [Required]
        public decimal? AcquisitionCost { get; set; }

        [Required]
        [StringLength(50)]
        public string? Sku { get; set; }

        [StringLength(50)]
        public string? Barcode { get; set; }
        [Required]
        public ProductStatus? ProductStatus { get; set; }
        [Required]
        [StringLength(50)]
        public string? PhotoName { get; set; }
        [Required]
        [StringLength(50)]
        public string? PhotoFilename { get; set; }
        [Required]
        public byte[]? PhotoBytes { get; set; }
        [Required]
        public long? StoreId { get; set; }
    }

    [LocalizerOfT<CreateProductModel>]
    public partial class CreateProductModelLocalizer
    {

    }
}
