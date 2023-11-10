using FairPlayShop.Common.CustomAttributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreModel>]
    public partial class CreateStoreModelLocalizer
    {
    }
}
