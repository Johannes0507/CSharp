using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelApp;

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Get AzureOpenAIOptions from appsettings.json
var azureOpenAIOptions = configuration.GetSection("AzureOpenAI").Get<AzureOpenAIOptions>();

# region Test to Connect AzureOpenAi API  
// // Create kernel
// var builder = Kernel.CreateBuilder();
// // Add a text or chat completion service using either:
// builder.Services.AddAzureOpenAIChatCompletion(
//     "Resuorce Name",
//     "Endpoint",
//     "Secret Key",
//     "Model Name");

// var kernel = builder.Build();

// var result = await kernel.InvokePromptAsync(
//         "Give me a list of breakfast foods with eggs and cheese");

// Console.WriteLine(result);
#endregion

#region TimePlugin Sample
// Sementic Kernel TimePlugin Sample
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.Services.AddAzureOpenAIChatCompletion(
    azureOpenAIOptions.ResourceName,
    azureOpenAIOptions.Endpoint,
    azureOpenAIOptions.ApiKey,
    azureOpenAIOptions.DeploymentModel
    );
kernelBuilder.Plugins.AddFromType<TimePlugin>();

var kernel = kernelBuilder.Build();

var currentDay = await kernel.InvokeAsync("TimePlugin", "DayOfWeek");
Console.WriteLine(currentDay);

#endregion

