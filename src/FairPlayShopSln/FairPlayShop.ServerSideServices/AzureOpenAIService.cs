using Azure.AI.OpenAI;
using FairPlayShop.Interfaces.Services;
using FairPlayShop.Models.AzureOpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FairPlayShop.ServerSideServices
{
    public class AzureOpenAIService(OpenAIClient openAIClient) : IAzureOpenAIService
    {
        public async Task<TranslationResponse?> TranslateSimpleTextAsync(string textToTranslate, string sourceLocale, string destLocale,
            CancellationToken cancellationToken)
        {
            TranslationRequest translationRequest = new TranslationRequest()
            {
                OriginalText = textToTranslate,
                SourceLocale = sourceLocale,
                DestLocale = destLocale
            };
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are an expert translator. Your jobs is to translate the text I give you." +
                    "My requests will be in json format with the following properties:" +
                    $"{nameof(TranslationRequest.OriginalText)}, {nameof(TranslationRequest.SourceLocale)}, {nameof(TranslationRequest.DestLocale)}" +
                    "Your responses must be in json format with the following properties:" +
                    $"{nameof(TranslationResponse.SourceLocale)}, {nameof(TranslationResponse.DestLocale)}, {nameof(TranslationResponse.TranslatedText)}"),
                    new ChatMessage(ChatRole.User, JsonSerializer.Serialize(translationRequest))
                }
            };
            var response = await openAIClient.GetChatCompletionsAsync("translationschat",
                chatCompletionsOptions, cancellationToken: cancellationToken);
            var contentResponse = 
            response.Value.Choices[0].Message.Content;
            TranslationResponse? translationResponse =
                JsonSerializer.Deserialize<TranslationResponse>(contentResponse);
            return translationResponse;
        }

        public async Task<TranslationResponse[]?> TranslateMultipleTextsAsync(
            TranslationRequest[] textsToTranslate, CancellationToken cancellationToken)
        {
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are an expert translator. Your jobs is to translate the text I give you." +
                    "My requests will be in json format with the following properties:" +
                    $"{nameof(TranslationRequest.OriginalText)}, {nameof(TranslationRequest.SourceLocale)}, {nameof(TranslationRequest.DestLocale)}" +
                    "Your responses must be in json format with the following properties:" +
                    $"{nameof(TranslationResponse.OriginalText)}, {nameof(TranslationResponse.SourceLocale)}, {nameof(TranslationResponse.DestLocale)}, {nameof(TranslationResponse.TranslatedText)}"),
                    new ChatMessage(ChatRole.User, JsonSerializer.Serialize(textsToTranslate))
                }
            };
            var response = await openAIClient.GetChatCompletionsAsync("translationschat",
                chatCompletionsOptions, cancellationToken: cancellationToken);
            var contentResponse =
            response.Value.Choices[0].Message.Content;
            TranslationResponse[]? translationResponse =
                JsonSerializer.Deserialize<TranslationResponse[]>(contentResponse);
            return translationResponse;
        }
    }

}
