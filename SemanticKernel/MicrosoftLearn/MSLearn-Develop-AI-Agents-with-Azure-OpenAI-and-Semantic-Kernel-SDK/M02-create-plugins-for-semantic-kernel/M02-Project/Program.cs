using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;

string yourDeploymentName = "Semantic-Kernel_Test";
string yourEndpoint = "https://semantickernel.openai.azure.com/";
string yourKey = "2bc304e82df5497486d08f9b67cede41";

var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(
    yourDeploymentName,
    yourEndpoint,
    yourKey,
    "pt-35-turbo-16k");

// Add ConversationSummaryPlugin
builder.Plugins.AddFromType<ConversationSummaryPlugin>();
var kernel = builder.Build();

#region Basic Protice
#region ConversationSummaryPlugin Basic Usage
// // Setting the prompt
// string input = @"I'm a vegan in search of new recipes. 
// I love spicy food! Can you give me a list of breakfast 
// recipes that are vegan friendly?";

// var result = await kernel.InvokeAsync(
//     "ConversationSummaryPlugin", 
//     "GetConversationActionItems", 
//     new() {{ "input", input }});

// Console.WriteLine(result);
#endregion

#region ConversationSummary Add User Background
// string history = @"In the heart of my bustling kitchen, I have embraced 
//     the challenge of satisfying my family's diverse taste buds and 
//     navigating their unique tastes. With a mix of picky eaters and 
//     allergies, my culinary journey revolves around exploring a plethora 
//     of vegetarian recipes.

//     One of my kids is a picky eater with an aversion to anything green, 
//     while another has a peanut allergy that adds an extra layer of complexity 
//     to meal planning. Armed with creativity and a passion for wholesome 
//     cooking, I've embarked on a flavorful adventure, discovering plant-based 
//     dishes that not only please the picky palates but are also heathy and 
//     delicious.";

// string functionPrompt = @"User background: 
//     {{ConversationSummaryPlugin.SummarizeConversation $history}}
//     Given this user's background, provide a list of relevant recipes.";
    
// var suggestRecipes = kernel.CreateFunctionFromPrompt(functionPrompt);
// var result = await kernel.InvokeAsync(suggestRecipes,
//     new KernelArguments() {
//         { "history", history }
//     });

// Console.WriteLine(result);
#endregion

#region (Advanced Usage) <message role=""> with assistant、user 
// string input = @"I'm planning an anniversary trip with my 
//     spouse. We like hiking, mountains, and beaches. Our 
//     travel budget is $15000";

// // 透過 <message role=""> 在 role 搭配填入 "assistant" 、 "user"
// string prompt = @$"
//     The following is a conversation with an AI travel assistant.
//     The assistant is helpful, creative, and very friendly.
    
//     <message role=""user""> 
//     Can you give me some travel destination suggestions?
//     <message>

//     <message role=""assistant"">
//     Of course! Do you have any budget or any specific activities in mind?
//     <message>

//     <message role=""user"">{input}</message>";

// var result = await kernel.InvokePromptAsync(prompt);
// Console.WriteLine(result);
#endregion

#region (Advanced Usage) <message role=""> with assistant、user、system

// string input = @"I have a vacation from June 1 to July 
// 22. I want to go to Greece. I live in Chicago.";

// string prompt = 
//     @$"<message role=""system"">
//         Instructions: Identify the from and to destinations and dates from the user's request.
//     </message>

//     <message role=""user"">
//         Can you give me a list of flights from settle to Tokyo?
//         I want to travel on March 11th to March 18th.  
//     </message>

//     <message role=""assistant"">
//         Seattle|Tokyo|03/11/2024|03/18/2024
//     </message>

//     <message role=""user"">{input}</message>";

// var result = await kernel.InvokePromptAsync(prompt);
// Console.WriteLine(result);

#endregion
#endregion

#region Travel Assistant (main)
var prompts = kernel.ImportPluginFromPromptDirectory("Prompts/TravelPlugins");

ChatHistory history = [];
string input = @"I'm planning an anniversary trip with my spouse.
    We like hiking, mountains, and beaches. 
    Our travel budget is $15000.";

var result = await kernel.InvokeAsync<string>(prompts["SuggestDestinations"],
    new() {
        { "input", input },
    }
);

Console.WriteLine(result);
history.AddUserMessage(input);
history.AddAssistantMessage(result);

Console.WriteLine("Where would you like to go?");
input = Console.ReadLine();

result = await kernel.InvokeAsync<string>(prompts["SuggestActivites"],
    new() {
        { "history", history },
        { "destination", input }
    }
);

Console.WriteLine(result);
#endregion