using FairPlayShop.Models.StoreCustomerOrderDetail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomerOrder
{
    public class CreateStoreCustomerOrderModel
    {
        [Required]
        public long? StoreCustomerId { get; set; }
        public decimal OrderSubTotal => CreateStoreCustomerOrderDetailModel!.Sum(p => p.LineTotal.GetValueOrDefault(0));
        public decimal OrderTotal => OrderSubTotal + TaxTotal;
        public decimal TaxTotal => OrderSubTotal * ((decimal)13 / 100);
        [Required]
        [ValidateComplexType]
        [Length(minimumLength:1, maximumLength:50, ErrorMessage = "Order must have between {1} and {2} lines")]
        public List<CreateStoreCustomerOrderDetailModel>? CreateStoreCustomerOrderDetailModel { get; set; }
    }
}
