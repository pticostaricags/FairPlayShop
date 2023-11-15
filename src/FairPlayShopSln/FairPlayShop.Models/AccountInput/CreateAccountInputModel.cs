using FairPlayShop.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.AccountInput
{
    public class CreateAccountInputModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateAccountInputModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountInputModelLocalizer.Email_Required))]
        [EmailAddress(ErrorMessageResourceType = typeof(CreateAccountInputModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountInputModelLocalizer.Email_EmailAddress))]
        [Display(ResourceType = typeof(CreateAccountInputModelLocalizer),
            Name = nameof(CreateAccountInputModelLocalizer.Email))]
        public string Email { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(CreateAccountInputModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountInputModelLocalizer.Password_Required))]
        [StringLength(100, ErrorMessageResourceType = typeof(CreateAccountInputModelLocalizer),
            ErrorMessageResourceName = nameof(CreateAccountInputModelLocalizer.PasswordStringLengthWithMin),
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType =typeof(CreateAccountInputModelLocalizer),
            Name =nameof(CreateAccountInputModelLocalizer.Password))]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(ResourceType =typeof(CreateAccountInputModelLocalizer),
            Name =nameof(CreateAccountInputModelLocalizer.ConfirmPassword))]
        [Compare("Password", ErrorMessageResourceType =typeof(CreateAccountInputModelLocalizer),
            ErrorMessageResourceName =nameof(CreateAccountInputModelLocalizer.ConfirmPassword_Compare))]
        public string ConfirmPassword { get; set; } = "";
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateAccountInputModel>]
    public partial class CreateAccountInputModelLocalizer
    {
        public static string Email => Localizer[EmailTextKey];
        public static string Password => Localizer[PasswordTextKey];
        public static string ConfirmPassword => Localizer[ConfirmPasswordTextKey];
        public static string ConfirmPassword_Compare => Localizer[ConfirmPasswordCompareTextKey];
        public static string PasswordStringLengthWithMin => Localizer[PasswordStringLengthWithMinTextKey];
        [ResourceKey(defaultValue: "Email")]
        public const string EmailTextKey = "EmailText";
        [ResourceKey(defaultValue: "{0} must be at least {2} and at max {1} characters long.")]
        public const string PasswordStringLengthWithMinTextKey = "PasswordStringLengthWithMinText";
        [ResourceKey(defaultValue: "Password")]
        public const string PasswordTextKey = "PasswordText";
        [ResourceKey(defaultValue: "Confirm password")]
        public const string ConfirmPasswordTextKey = "ConfirmPasswordText";
        [ResourceKey(defaultValue: "The password and confirmation password do not match.")]
        public const string ConfirmPasswordCompareTextKey = "ConfirmPasswordCompareText";
    }
}
