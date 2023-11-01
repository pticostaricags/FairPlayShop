using FairPlayShop.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomerAddress
{
    public class CreateStoreCustomerAddressModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Firstname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Firstname_StringLength))]
        public string? Firstname { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Lastname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Lastname_StringLength))]
        public string? Lastname { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Company_StringLength))]
        public string? Company { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine1_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine1_StringLength))]
        public string? AddressLine1 { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine2_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine2_StringLength))]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.CityId_Required))]
        public int? CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PostalCode_Required))]
        [StringLength(10, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PostalCode_StringLength))]
        public string? PostalCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PhoneNumber_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PhoneNumber_StringLength))]
        [Phone(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PhoneNumber_Phone))]
        public string? PhoneNumber { get; set; }
    }

    [LocalizerOfT<CreateStoreCustomerAddressModel>]
    public partial class CreateStoreCustomerAddressModelLocalizer
    {

    }
}
