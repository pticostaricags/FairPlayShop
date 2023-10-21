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
    public class StoreCustomerOrderService(
        IUserProviderService userProviderService, FairPlayShopDatabaseContext fairPlayShopDatabaseContext)
        : IStoreCustomerOrderService
    {
        public async Task CreateStoreCustomerOrderAsync(CreateStoreCustomerOrderModel createStoreCustomerOrderModel, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            StoreCustomerOrder storeCustomerOrder = new()
            {
                StoreCustomerId = createStoreCustomerOrderModel.StoreCustomerId!.Value,
                OrderDateTime = DateTimeOffset.UtcNow,
                OrderSubTotal = createStoreCustomerOrderModel.OrderSubTotal,
                OrderTotal = createStoreCustomerOrderModel.OrderTotal,
                TaxTotal = createStoreCustomerOrderModel.TaxTotal
            };
            await fairPlayShopDatabaseContext.StoreCustomerOrder.AddAsync(
                storeCustomerOrder, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}
