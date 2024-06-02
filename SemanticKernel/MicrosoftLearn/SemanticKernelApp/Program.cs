
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

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

// #region TimePlugin
// Sementic Kernel TimePlugin Sample
var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.Services.AddAzureOpenAIChatCompletion(
    "Semantic-Kernel_Test",
    "https://semantickernel.openai.azure.com/",
    "2bc304e82df5497486d08f9b67cede41",
    "gpt-35-turbo-16k");

kernelBuilder.Plugins.AddFromType<TimePlugin>();
var kernel = kernelBuilder.Build();
var currentDay = await kernel.InvokeAsync("TimePlugin", "DayOfWeek");
Console.WriteLine(currentDay);



