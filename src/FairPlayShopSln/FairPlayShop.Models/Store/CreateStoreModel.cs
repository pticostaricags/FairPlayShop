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
        [Required(ErrorMessageResourceType = typeof(CreateStoreModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreModelLocalizer.Name_Required))]
        [StringLength(50, ErrorMessageResourceType =typeof(CreateStoreModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreModelLocalizer.Name_StringLength))]
        public string? Name { get; set; }
    }

    public class CreateStoreModelLocalizer
    {
        public static string? Name_Required { get; set; } = "{0} is required";
        public static string? Name_StringLength { get; set; } = "{0} must have a maximum of {1} characters";
    }
}
