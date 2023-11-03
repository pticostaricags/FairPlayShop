using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StoreCustomerOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    [ServerSideServiceOfT<DataAccess.Models.StoreCustomerOrder>]
    public class StoreCustomerOrderService(FairPlayShopDatabaseContext fairPlayShopDatabaseContext)
        : IStoreCustomerOrderService
    {
        public async Task CreateStoreCustomerOrderAsync(CreateStoreCustomerOrderModel createStoreCustomerOrderModel, CancellationToken cancellationToken)
        {
            StoreCustomerOrder storeCustomerOrder = new()
            {
                StoreCustomerId = createStoreCustomerOrderModel.StoreCustomerId!.Value,
                OrderDateTime = DateTimeOffset.UtcNow,
                OrderSubTotal = createStoreCustomerOrderModel.OrderSubTotal,
                OrderTotal = createStoreCustomerOrderModel.OrderTotal,
                TaxTotal = createStoreCustomerOrderModel.TaxTotal,
            };
            foreach (var singleOrderLine in createStoreCustomerOrderModel.CreateStoreCustomerOrderDetailModel!)
            {
                storeCustomerOrder.StoreCustomerOrderDetail.Add(new StoreCustomerOrderDetail() 
                {
                    LineTotal = singleOrderLine.LineTotal!.Value,
                    Quantity = singleOrderLine.Quantity!.Value,
                    ProductId = singleOrderLine.ProductId!.Value,
                    UnityPrice = singleOrderLine.UnitPrice!.Value
                });
            }
            await fairPlayShopDatabaseContext.StoreCustomerOrder.AddAsync(
                storeCustomerOrder, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}
