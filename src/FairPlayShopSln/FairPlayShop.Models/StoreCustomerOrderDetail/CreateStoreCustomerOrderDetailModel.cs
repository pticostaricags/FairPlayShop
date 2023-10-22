using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomerOrderDetail
{
    public class CreateStoreCustomerOrderDetailModel
    {
        [Required]
        public long? StoreCustomerOrderId { get; set; }
        [Required]
        public long? ProductId { get; set; }
        [Required]
        public decimal? UnitPrice { get; set; }
        [Required]
        public decimal? Quantity { get; set; }
        public decimal? LineTotal => UnitPrice.GetValueOrDefault() * Quantity.GetValueOrDefault(0);
    }
}
