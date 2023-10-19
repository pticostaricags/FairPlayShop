using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayShop.Common.Enums;

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
        [Column("SKU")]
        [StringLength(50)]
        public string? Sku { get; set; }

        [Required]
        [StringLength(50)]
        public string? Barcode { get; set; }
        [Required]
        public ProductStatus? ProductStatus { get; set; }
    }
}
