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

        [Required]
        [StringLength(50)]
        public string? Firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string? Lastname { get; set; }

        [Required]
        [StringLength(50)]
        public string? Surname { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string? PhoneNumber { get; set; }
    }
}
