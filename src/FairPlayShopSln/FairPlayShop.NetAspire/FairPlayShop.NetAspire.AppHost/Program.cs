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
builder.Build().Run();
