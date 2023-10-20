﻿using FairPlayShop.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Interfaces.Services
{
    public interface IStoreService
    {
        Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken);
        Task<StoreModel[]?> GetMyStoreListAsync(CancellationToken cancellationToken);
    }
}