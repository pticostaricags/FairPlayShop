﻿using FairPlayShop.Common.CustomAttributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FairPlayShop.Models.StoreCustomerAddress
{
    public class CreateStoreCustomerAddressModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Name_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Name_StringLength))]
        public string? Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.FirstSurname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.FirstSurname_StringLength))]
        public string? FirstSurname { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.SecondSurname_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.SecondSurname_StringLength))]
        public string? SecondSurname { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.Company_StringLength))]
        public string? Company { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine1_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine1_StringLength))]
        public string? AddressLine1 { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine2_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.AddressLine2_StringLength))]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.CityId_Required))]
        public int? CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PostalCode_Required))]
        [StringLength(10, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PostalCode_StringLength))]
        public string? PostalCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PhoneNumber_Required))]
        [StringLength(50, ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PhoneNumber_StringLength))]
        [Phone(ErrorMessageResourceType = typeof(CreateStoreCustomerAddressModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerAddressModelLocalizer.PhoneNumber_Phone))]
        public string? PhoneNumber { get; set; }
    }

    [CompilerGenerated]
    [LocalizerOfT<CreateStoreCustomerAddressModel>]
    public partial class CreateStoreCustomerAddressModelLocalizer
    {

    }
}
