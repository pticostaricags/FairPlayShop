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
        [Required]
        [StringLength(50)]
        public string? Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string? Lastname { get; set; }

        [StringLength(50)]
        public string? Company { get; set; }

        [Required]
        [StringLength(50)]
        public string? AddressLine1 { get; set; }

        [Required]
        [StringLength(50)]
        public string? AddressLine2 { get; set; }

        [Required]
        public int? CityId { get; set; }

        [Required]
        [StringLength(10)]
        public string? PostalCode { get; set; }

        [Required]
        [StringLength(50)]
        public string? PhoneNumber { get; set; }
    }
}
