﻿using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Models.StoreCustomerAddress;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FairPlayShop.Models.StoreCustomer
{
    public class CreateStoreCustomerModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.StoreId_Required))]
        public long? StoreId { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.Name_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.Name_StringLength))]
        public string? Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.FirstSurname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.FirstSurnameStringLengthTextKey))]
        public string? FirstSurname { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.SecondSurname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.SecondSurnameStringLengthTextKey))]
        public string? SecondSurname { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.EmailAddress_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.EmailAddress_StringLength))]
        [EmailAddress(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.EmailAddress_EmailAddress))]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.PhoneNumber_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.PhoneNumber_StringLength))]
        [Phone(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.PhoneNumber_Phone))]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerModelLocalizer.CreateStoreCustomerAddressModel_Required))]
        [ValidateComplexType]
        public CreateStoreCustomerAddressModel? CreateStoreCustomerAddressModel { get; set; }
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreCustomerModel>]
    public partial class CreateStoreCustomerModelLocalizer
    {

    }
}
