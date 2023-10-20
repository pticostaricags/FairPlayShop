using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.Store
{
    public class CreateStoreModel
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
    }
}
