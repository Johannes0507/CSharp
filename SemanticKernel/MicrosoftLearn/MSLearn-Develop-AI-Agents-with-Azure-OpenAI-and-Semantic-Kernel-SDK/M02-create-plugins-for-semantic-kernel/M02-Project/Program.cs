using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

string yourDeploymentName = "Semantic-Kernel";
string yourEndpoint = "https://semantickernel.openai.azure.com/";
string yourKey = "";

var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(
    yourDeploymentName,
    yourEndpoint,
    yourKey,
    "gpt-4");

// Add ConversationSummaryPlugin
builder.Plugins.AddFromType<ConversationSummaryPlugin>();
var kernel = builder.Build();

// Setting the prompt
string input = @"I'm a vegan in search of new recipes. 
I love spicy food! Can you give me a list of breakfast 
recipes that are vegan friendly?";

var result = await kernel.InvokeAsync(
    "ConversationSummaryPlugin", 
    "GetConversationActionItems", 
    new() {{ "input", input }});
    
Console.WriteLine(result);