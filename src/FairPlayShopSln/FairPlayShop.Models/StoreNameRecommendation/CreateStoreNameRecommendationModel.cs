using FairPlayShop.Common.CustomAttributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FairPlayShop.Models.StoreNameRecommendation
{
    public class CreateStoreNameRecommendationModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreNameRecommendationModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreNameRecommendationModelLocalizer.Products_Required))]
        public string? Products { get; set; }
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreNameRecommendationModel>]
    public partial class CreateStoreNameRecommendationModelLocalizer
    {

    }
}
