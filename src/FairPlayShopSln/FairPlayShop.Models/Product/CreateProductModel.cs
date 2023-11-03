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
        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Name_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Name_StringLength))]
        public string? Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Description_Required))]
        [StringLength(500, ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Description_StringLength))]
        public string? Description { get; set; }
        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.QuantityInStock_Required))]
        public int? QuantityInStock { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.SellingPrice_Required))]
        public decimal? SellingPrice { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.AcquisitionCost_Required))]
        public decimal? AcquisitionCost { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Sku_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Sku_StringLength))]
        public string? Sku { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.Barcode_StringLength))]
        public string? Barcode { get; set; }
        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.ProductStatus_Required))]
        public ProductStatus? ProductStatus { get; set; }
        [Required(ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.PhotoName_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName = nameof(CreateProductModelLocalizer.PhotoName_StringLength))]
        public string? PhotoName { get; set; }
        [Required(ErrorMessageResourceType =typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName =nameof(CreateProductModelLocalizer.PhotoFilename_Required))]
        [StringLength(50,ErrorMessageResourceType =typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName =nameof(CreateProductModelLocalizer.PhotoFilename_StringLength))]
        public string? PhotoFilename { get; set; }
        [Required(ErrorMessageResourceType =typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName =nameof(CreateProductModelLocalizer.PhotoBytes_Required))]
        public byte[]? PhotoBytes { get; set; }
        [Required(ErrorMessageResourceType =typeof(CreateProductModelLocalizer),
            ErrorMessageResourceName =nameof(CreateProductModelLocalizer.StoreId_Required))]
        public long? StoreId { get; set; }
    }

    [LocalizerOfT<CreateProductModel>]
    public partial class CreateProductModelLocalizer
    {

    }
}
