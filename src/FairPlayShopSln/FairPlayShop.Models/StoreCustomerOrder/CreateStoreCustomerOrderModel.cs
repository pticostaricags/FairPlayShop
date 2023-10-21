using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomerOrder
{
    public class CreateStoreCustomerOrderModel
    {
        [Required]
        public long? StoreCustomerId { get; set; }
        public decimal OrderSubTotal { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal TaxTotal { get; set; }
    }
}
