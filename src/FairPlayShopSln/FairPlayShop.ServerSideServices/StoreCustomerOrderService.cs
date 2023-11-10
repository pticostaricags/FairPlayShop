using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Common.CustomExceptions;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.StoreCustomerOrder;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.ServerSideServices
{
    [ServerSideServiceOfT<StoreCustomerOrder>]
    public class StoreCustomerOrderService(
        IUserProviderService userProviderService,
        IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory)
        : IStoreCustomerOrderService
    {
        public async Task CreateStoreCustomerOrderAsync(CreateStoreCustomerOrderModel createStoreCustomerOrderModel, CancellationToken cancellationToken)
        {
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            var executionStrategy = fairPlayShopDatabaseContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await fairPlayShopDatabaseContext.Database.BeginTransactionAsync(cancellationToken: cancellationToken);
                try
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
                        var productEntity = await fairPlayShopDatabaseContext
                            .Product.SingleAsync(p => p.ProductId == singleOrderLine.ProductId,
                            cancellationToken: cancellationToken);
                        if (productEntity.QuantityInStock < singleOrderLine.Quantity)
                            throw new RuleException($"There are only {productEntity.QuantityInStock} items of {productEntity.Name} left in stock. Please modify your order or try again later. ");
                        productEntity.QuantityInStock -= (int)singleOrderLine.Quantity!.Value;
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
                    await transaction.CommitAsync(cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken: cancellationToken);
                    throw;
                }
            });
        }

        public async Task<StoreCustomerOrderModel[]?> GetStoreCustomerOrderListAsync(long storeId, CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            bool storeFounder = await dbContext.Store.AnyAsync(p => p.StoreId == storeId &&
            p.OwnerId == userId, cancellationToken: cancellationToken);
            if (!storeFounder)
                return null;
            var result = await dbContext.StoreCustomerOrder.Include(p => p.StoreCustomer)
                .ThenInclude(p => p.Store)
                .Where(p => p.StoreCustomer.StoreId == storeId)
                .Select(p => new StoreCustomerOrderModel()
                {
                    OrderSubTotal = p.OrderSubTotal,
                    TaxTotal = p.TaxTotal,
                    OrderTotal = p.OrderTotal,
                    OrderDateTime = p.OrderDateTime,
                    StoreCustomerName = p.StoreCustomer.Name,
                    StoreCustomerFirstSurname = p.StoreCustomer.FirstSurname,
                    StoreCustomerSecondSurname = p.StoreCustomer.SecondSurname
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
