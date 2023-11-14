using Azure.AI.OpenAI;
using FairPlayShop.Common;
using FairPlayShop.Common.CustomAttributes;
using FairPlayShop.Common.CustomExceptions;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.Pagination;
using FairPlayShop.Models.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace FairPlayShop.ServerSideServices
{
    public class StoreService(IUserProviderService userProviderService,
        IDbContextFactory<FairPlayShopDatabaseContext> dbContextFactory,
        ILogger<StoreService> logger,
        IStringLocalizer<StoreService> localizer,
        OpenAIClient openAIClient) : IStoreService
    {
        public async Task CreateMyStoreAsync(CreateStoreModel createStoreModel, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Executing {{ {nameof(createStoreModel)} }}", createStoreModel);
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            if (await fairPlayShopDatabaseContext.Store.AnyAsync(p => p.Name == createStoreModel.Name,
                cancellationToken: cancellationToken))
            {
                string message =
                    String.Format(
                    localizer[StoreNameExistTextKey], createStoreModel.Name);
                throw new RuleException(message);
            }
            var userId = userProviderService.GetCurrentUserId();
            Store entity = new()
            {
                Name = createStoreModel.Name,
                OwnerId = userId,
            };
            await fairPlayShopDatabaseContext.Store.AddAsync(entity, cancellationToken: cancellationToken);
            await fairPlayShopDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task<PaginationOfT<StoreModel>> GetPaginatedMyStoreListAsync(
            PaginationRequest paginationRequest, CancellationToken cancellationToken)
        {
            PaginationOfT<StoreModel> result = new();
            var userId = userProviderService.GetCurrentUserId();
            using var fairPlayShopDatabaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken: cancellationToken);
            string orderByString = string.Empty;
            if (paginationRequest.SortingItems?.Length > 0)
                orderByString =
                    String.Join(",",
                    paginationRequest.SortingItems.Select(p => $"{p.PropertyName} {GetSortTypeString(p.SortType)}"));
            var query = fairPlayShopDatabaseContext.Store.AsNoTracking()
                .Where(p => p.OwnerId == userId)
                .Select(p => new StoreModel()
                {
                    StoreId = p.StoreId,
                    Name = p.Name,
                    CustomerCount = p.StoreCustomer.Count
                });
            if (!String.IsNullOrEmpty(orderByString))
                query = query.OrderBy(orderByString);
            result.TotalItems = await query.CountAsync(cancellationToken: cancellationToken);
            result.TotalPages = (int)Math.Ceiling((decimal)result.TotalItems /
                Constants.Pagination.PageSize);
            result.PageSize = Constants.Pagination.PageSize;
            result.Items = await query.Skip(paginationRequest.StartIndex).Take(Constants.Pagination.PageSize)
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        private static string GetSortTypeString(SortType sortType)
        {
            return sortType == SortType.Ascending ? "ASC" : "DESC";
        }

        public async Task<string> GetStoreRecommendedNameAsync(string[] storeProducts, string[]? namesToExclude, CancellationToken cancellationToken)
        {
            string systemMessage = $"You will take the role of an expert in entrepreneurship, " +
                $"startups, and creation of succesful businesses of all sizes. " +
                $"Your job is to give me a list of 100 recommended names for my new store. " +
                $"Please research the availability of these names and check if they are trademarked or being used by other businesses to avoid any legal issues. " +
                $"The recommended names must refer to all products, not only one." +
                $"Give me the name availability and trademark information." +
                $"Give me the response in HTML 5 table format and in the following language locale: \"{CultureInfo.CurrentCulture.Name}\"";
            string userMessage = $"Products in my store: {String.Join(",", storeProducts)}.";
            if (namesToExclude?.Length > 0)
            {
                userMessage = $"{userMessage} \r\nNames to exclude: {String.Join(",", namesToExclude)}";
            }
            ChatCompletionsOptions chatCompletionsOptions = new()
            {
                DeploymentName = "translationschat",
                Messages =
                {
                    new ChatMessage(ChatRole.System, systemMessage),
                    new ChatMessage(ChatRole.User, userMessage)
                }
            };
            var response = await openAIClient.GetChatCompletionsAsync(
                chatCompletionsOptions, cancellationToken: cancellationToken);
            var contentResponse =
            response.Value.Choices[0].Message.Content;
            return contentResponse;
        }

        public async Task<Uri[]> GetRecommendedLogoAsync(string[] storeProducts, CancellationToken cancellationToken)
        {
            string prompt = $"Logo for an online store selling the following products: {String.Join(",", storeProducts)}";

            var response = await openAIClient!.GetImageGenerationsAsync(
                imageGenerationOptions: new(prompt)
                {
                    ImageCount = 4,
                    Size = ImageSize.Size1024x1024,
                }, cancellationToken:cancellationToken);

            return response.Value.Data.Select(p=>p.Url).ToArray();
        }

        [ResourceKey(defaultValue: "There is already a store named '{0}'. Plaese use another")]
        public const string StoreNameExistTextKey = "StoreNameExistText";
    }
}
