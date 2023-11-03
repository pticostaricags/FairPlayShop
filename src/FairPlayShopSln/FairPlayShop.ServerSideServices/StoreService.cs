﻿using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class StoreService(IUserProviderService userProviderService,
        FairPlayShopDatabaseContext fairPlayShopDatabaseContext,
        ILogger<StoreService> logger) : IStoreService
    {
        public async Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken)
        {
            Convert.tore
            logger.LogInformation($"Executing {{{nameof(createStoreModel)}}}", createStoreModel);
            var userId = userProviderService.GetCurrentUserId();
            Store entity = new Store()
            {
                Name = createStoreModel.Name,
                OwnerId = userId,
            };
            await fairPlayShopDatabaseContext.Store.AddAsync(entity, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<StoreModel[]?> GetMyStoreListAsync(CancellationToken cancellationToken)
        {
            var userId = userProviderService.GetCurrentUserId();
            var result = await fairPlayShopDatabaseContext.Store
                .Where(p=>p.OwnerId == userId)
                .AsNoTracking()
                .Select(p => new StoreModel() 
                {
                    StoreId = p.StoreId,
                    Name = p.Name,
                    CustomerCount = p.StoreCustomer.Count
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
