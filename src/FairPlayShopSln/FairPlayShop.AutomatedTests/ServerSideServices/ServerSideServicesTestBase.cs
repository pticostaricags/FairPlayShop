﻿using FairPlayShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    public abstract class ServerSideServicesTestBase
    {
        public static string? CurrentUserId { get; protected set; }
        public static readonly MsSqlContainer _msSqlContainer
        = new MsSqlBuilder().Build();
        protected static async Task<FairPlayShopDatabaseContext> GetFairPlayBudgetDatabaseContextAsync()
        {
            DbContextOptionsBuilder<FairPlayShopDatabaseContext> dbContextOptionsBuilder =
                new();
            dbContextOptionsBuilder.UseSqlServer(_msSqlContainer.GetConnectionString(),
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(3),
                            errorNumbersToAdd: null);
                });
            FairPlayShopDatabaseContext fairPlayShopDatabaseContext =
                new(dbContextOptionsBuilder.Options);
            await fairPlayShopDatabaseContext.Database.EnsureCreatedAsync();
            await fairPlayShopDatabaseContext.Database.ExecuteSqlRawAsync(Properties.Resources.SeedData);
            return fairPlayShopDatabaseContext;
        }

        private static TestUserProviderService GetUserProviderService()
        {
            return new TestUserProviderService();
        }
    }
}
