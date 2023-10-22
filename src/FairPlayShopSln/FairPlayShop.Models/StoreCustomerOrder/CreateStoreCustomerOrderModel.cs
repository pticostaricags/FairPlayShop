using FairPlayShop.Models.StoreCustomerOrderDetail;
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
        public decimal OrderSubTotal => CreateStoreCustomerOrderDetailModel!.Sum(p => p.LineTotal.GetValueOrDefault(0));
        public decimal OrderTotal { get; set; }
        public decimal TaxTotal { get; set; }
        [Required]
        [ValidateComplexType]
        public List<CreateStoreCustomerOrderDetailModel>? CreateStoreCustomerOrderDetailModel { get; set; }
    }
}
