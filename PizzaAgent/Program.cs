using System.ClientModel;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Octokit;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using PizzaAgent;

string gitHubAppName = "xnavigator";
string githubCopilotCompletionsUrl = "https://api.githubcopilot.com/chat/completions";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Welcome to the GitHub Copilot Pizza Agent");

app.MapPost("/agent", async (
    [FromHeader(Name = "X-GitHub-Token")] string githubToken, 
    [FromBody] ChatRequest userRequest) =>
{
    var octokitClient = 
        new GitHubClient(
            new Octokit.ProductHeaderValue(gitHubAppName))
        {
            Credentials = new Credentials(githubToken)
        };
    var user = await octokitClient.User.Current();
    
    var openApiClient = new OpenAIClient(new ApiKeyCredential(githubToken),new OpenAIClientOptions { Endpoint = new Uri(githubCopilotCompletionsUrl)});
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddOpenAIChatCompletion("gpt-4o", openApiClient);
    Kernel kernel = kernelBuilder.Build();
    
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    
    // userRequest.Messages.Insert(0, new ChatMessage
    // {
    //     Role = ChatRole.System,
    //     Text = $"The user who is logged in is: @{user.Login}"
    // });
    // userRequest.Messages.Insert(0, new ChatMessage
    // {
    //     Role = ChatRole.System,
    //     Text = "You are a Pizza Agent. You can help feed the hungry developers with pizza by ordering pizzas and having them delivered to their location."
    // });

    ChatHistory chatHistory = [];
    chatHistory.AddSystemMessage($"The user who is logged in is: @{user.Login}");
    chatHistory.AddSystemMessage("You are a Pizza Agent. You can help feed the hungry developers with pizza by ordering pizzas and having them delivered to their location.");

    foreach (ChatMessage message in userRequest.Messages)
    {
        chatHistory.AddMessage(
            message.Role == ChatRole.System ? AuthorRole.System : AuthorRole.User,
            message.Text
        );
    }
    
    var reply = await chatCompletionService.GetChatMessageContentsAsync(chatHistory, kernel: kernel);
    return reply;
    
    // var httpClient = new HttpClient();
    // httpClient.DefaultRequestHeaders.Authorization = 
    //     new AuthenticationHeaderValue("Bearer", githubToken);
    // userRequest.Stream = true;
    //
    // var copilotLLMResponse = await httpClient.PostAsJsonAsync(
    //     githubCopilotCompletionsUrl, userRequest);
    //
    // var responseStream = await copilotLLMResponse.Content.ReadAsStreamAsync();
    // return Results.Stream(responseStream, "application/json");

});


app.MapGet("/callback", () => "You may close this tab and " + 
                              "return to GitHub.com (where you should refresh the page " +
                              "and start a fresh chat). If you're using VS Code or " +
                              "Visual Studio, return there.");

app.Run();