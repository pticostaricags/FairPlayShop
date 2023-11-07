﻿using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Models.StoreCustomerOrderDetail;
using System.ComponentModel.DataAnnotations;

namespace FairPlayShop.Models.StoreCustomerOrder
{
    public class CreateStoreCustomerOrderModel
    {
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderModelLocalizer.StoreCustomerId_Required))]
        public long? StoreCustomerId { get; set; }
        public decimal OrderSubTotal => CreateStoreCustomerOrderDetailModel!.Sum(p => p.LineTotal.GetValueOrDefault(0));
        public decimal OrderTotal => OrderSubTotal + TaxTotal;
        public decimal TaxTotal => OrderSubTotal * ((decimal)13 / 100);
        [Required(ErrorMessageResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderModelLocalizer.CreateStoreCustomerOrderDetailModel_Required))]
        [ValidateComplexType]
        [Length(minimumLength: 1, maximumLength: 50,
            ErrorMessageResourceType = typeof(CreateStoreCustomerOrderModelLocalizer),
            ErrorMessageResourceName = nameof(CreateStoreCustomerOrderModelLocalizer.CreateStoreCustomerOrderDetailModel_Length))]
        public List<CreateStoreCustomerOrderDetailModel>? CreateStoreCustomerOrderDetailModel { get; set; }
    }

    [LocalizerOfT<CreateStoreCustomerOrderModel>]
    public partial class CreateStoreCustomerOrderModelLocalizer
    {

    }
}
