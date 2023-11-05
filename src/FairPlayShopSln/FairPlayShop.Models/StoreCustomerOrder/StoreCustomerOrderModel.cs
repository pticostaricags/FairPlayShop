using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomerOrder
{
    public class StoreCustomerOrderModel
    {
        public decimal OrderSubTotal { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTimeOffset OrderDateTime { get; set; }
        public string? StoreCustomerName { get; set; }
        public string? StoreCustomerFirstSurname { get; set; }
        public string? StoreCustomerSecondSurname { get; set; }
        public decimal TaxTotal { get; set; }
    }
}
