using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Models.StoreCustomerAddress;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomer
{
    public class CreateStoreCustomerModel
    {
        [Required]
        public long? StoreId { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.Firstname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.Firstname_StringLength))]
        public string? Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string? Lastname { get; set; }

        [Required]
        [StringLength(50)]
        public string? Surname { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.EmailAddress_EmailAddress))]
        public string? EmailAddress { get; set; }

        [Required]
        [StringLength(50)]
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        [ValidateComplexType]
        public CreateStoreCustomerAddressModel? CreateStoreCustomerAddressModel { get; set; }
    }

    [LocalizerOfT<CreateStoreCustomerModel>]
    public partial class CreateStoreCustomerModelLocalizer
    {

    }
}
