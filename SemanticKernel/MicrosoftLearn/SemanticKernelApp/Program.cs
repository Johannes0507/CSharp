using Microsoft.SemanticKernel;

// Create kernel
var builder = Kernel.CreateBuilder();
// Add a text or chat completion service using either:
builder.Services.AddAzureOpenAIChatCompletion(
    "Resuorce Name",
    "Endpoint",
    "Secret Key",
    "Model Name");

var kernel = builder.Build();

var result = await kernel.InvokePromptAsync(
        "Give me a list of breakfast foods with eggs and cheese");

Console.WriteLine(result);
