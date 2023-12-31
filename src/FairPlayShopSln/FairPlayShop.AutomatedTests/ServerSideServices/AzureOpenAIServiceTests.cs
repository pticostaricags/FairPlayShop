﻿using Azure.AI.OpenAI;
using FairPlayShop.ServerSideServices;
using Microsoft.Extensions.Configuration;

namespace FairPlayShop.AutomatedTests.ServerSideServices
{
    [TestClass]
    public class AzureOpenAIServiceTests
    {
        [TestMethod]
        public async Task Test_Translate()
        {
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.AddUserSecrets<AzureOpenAIServiceTests>();
            var config = configurationBuilder.Build();
            var endpoint = config["AzureOpenAI:Endpoint"] ?? throw new Exception("Can't find config for AzureOpenAI:Endpoint");
            var key = config["AzureOpenAI:Key"] ?? throw new Exception("Can't find config for AzureOpenAI:Key");
            OpenAIClient openAIClient = new(new Uri(endpoint),
                new Azure.AzureKeyCredential(key));
            AzureOpenAIService azureOpenAIService =
                new(openAIClient);
            var sourceText = "Hello";
            var expectedTranslation = "Hola";
            var result = await azureOpenAIService.TranslateSimpleTextAsync(sourceText, "en-US",
                "es-CR", CancellationToken.None);
            Assert.AreEqual(expectedTranslation, result!.TranslatedText);
        }

        [TestMethod]
        public async Task Test_TranslateMultipleTextsAsync()
        {
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.AddUserSecrets<AzureOpenAIServiceTests>();
            var config = configurationBuilder.Build();
            var endpoint = config["AzureOpenAI:Endpoint"] ?? throw new Exception("Can't find config for AzureOpenAI:Endpoint");
            var key = config["AzureOpenAI:Key"] ?? throw new Exception("Can't find config for AzureOpenAI:Key");
            OpenAIClient openAIClient = new(new Uri(endpoint),
                new Azure.AzureKeyCredential(key));
            AzureOpenAIService azureOpenAIService =
                new(openAIClient);
            Models.AzureOpenAI.TranslationRequest[] translationRequest =
                [
                    new()
                    {
                        OriginalText = "Hello",
                        SourceLocale = "en-US",
                        DestLocale = "es-CR",
                    },
                    new Models.AzureOpenAI.TranslationRequest()
                    {
                        OriginalText = "Perro",
                        SourceLocale = "es-MX",
                        DestLocale = "en-US"
                    }
                ];
            var result = await azureOpenAIService
                .TranslateMultipleTextsAsync(translationRequest, CancellationToken.None);
            Assert.AreEqual("Hola", result![0].TranslatedText);
            Assert.AreEqual("Dog", result![1].TranslatedText);
        }
    }
}
