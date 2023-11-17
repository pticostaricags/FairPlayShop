using FairPlayShop.Common.CustomExceptions;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
var connectionString =
builder.Configuration.GetConnectionString("DefaultConnection") ??
throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var endpoint = builder.Configuration["AzureOpenAIEndpoint"] ?? throw new ConfigurationException("Can't find config for AzureOpenAI:Endpoint");
var key =
    builder.Configuration["AzureOpenAIKey"] ?? throw new ConfigurationException("Can't find config for AzureOpenAI:Key");
builder.AddProject<Projects.FairPlayShop>("fairplayshopweb")
    .WithEnvironment(callback =>
    {
        callback.EnvironmentVariables.Add("DefaultConnection", connectionString);
        callback.EnvironmentVariables.Add("AzureOpenAIKey", key);
        callback.EnvironmentVariables.Add("AzureOpenAIEndpoint", endpoint);
    });

AddDataImporter(builder, connectionString);
//AddTestDataGenerator(builder, connectionString);
builder.Build().Run();

static void AddTestDataGenerator(IDistributedApplicationBuilder builder, string connectionString)
{
    builder.AddProject<Projects.FairPlayShop_TestDataGenerator>("testdatagenerator")
        .WithEnvironment(callback =>
        {
            callback.EnvironmentVariables.Add("DefaultConnection", connectionString);
        });
}

static void AddDataImporter(IDistributedApplicationBuilder builder, string connectionString)
{
    builder.AddProject<Projects.FairPlayShop_DataImporter>("dataimporter")
        .WithEnvironment(callback =>
        {
            callback.EnvironmentVariables.Add("DefaultConnection", connectionString);
        });
}