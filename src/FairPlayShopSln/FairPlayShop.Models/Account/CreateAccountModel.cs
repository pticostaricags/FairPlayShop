using FairPlayShop.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.Account
{
    public partial class CreateAccountModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateAccountModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountModelLocalizer.Email_Required))]
        [EmailAddress(ErrorMessageResourceType = typeof(CreateAccountModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountModelLocalizer.Email_EmailAddress))]
        [Display(ResourceType = typeof(CreateAccountModelLocalizer),
            Name = nameof(CreateAccountModelLocalizer.Email))]
        public string Email { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(CreateAccountModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountModelLocalizer.Password_Required))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(CreateAccountModelLocalizer),
            Name = nameof(CreateAccountModelLocalizer.Password))]
        public string Password { get; set; } = "";

        [Display(ResourceType = typeof(CreateAccountModelLocalizer),
            Name = nameof(CreateAccountModelLocalizer.RememberMe))]
        public bool RememberMe { get; set; }
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateAccountModel>]
    public partial class CreateAccountModelLocalizer
    {
        public static string RememberMe => Localizer[RememberMeTextKey];
        public static string Email => Localizer[EmailTextKey];
        public static string Password => Localizer[PasswordTextKey];
        [ResourceKey(defaultValue: "Remember me?")]
        public const string RememberMeTextKey = "RememberMeText";
        [ResourceKey(defaultValue: "Email")]
        public const string EmailTextKey = "EmailText";
        [ResourceKey(defaultValue: "Password")]
        public const string PasswordTextKey = "PasswordText";
    }
}
