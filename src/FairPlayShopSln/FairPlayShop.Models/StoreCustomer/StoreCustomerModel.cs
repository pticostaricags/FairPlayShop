using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.StoreCustomer
{
    public class StoreCustomerModel
    {
        public long StoreCustomerId { get; set; }
        public long StoreId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Surname { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
