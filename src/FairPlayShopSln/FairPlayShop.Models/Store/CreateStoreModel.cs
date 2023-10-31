using FairPlayShop.Common.CustomAttributes;
using Microsoft.Extensions.Localization;
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
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreModelLocalizer.Name_StringLength))]
        public string? Name { get; set; }
    }

    [LocalizerOfT<CreateStoreModel>]
    public partial class CreateStoreModelLocalizer
    {
    }
}
