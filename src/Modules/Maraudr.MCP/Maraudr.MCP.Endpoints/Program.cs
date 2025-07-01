using Maraudr.MCP.Infrastructure;
using MCP.Maraudr.Application;

using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddMcpClient(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Add Chat Client
builder.Services.AddSingleton<IChatClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = configuration["OpenRouter:ApiKey"];
    var baseUrl = configuration["OpenRouter:BaseUrl"];
    var modelName = configuration["OpenRouter:ModelName"];

    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(modelName))
    {
        throw new InvalidOperationException("OpenRouter ApiKey or ModelName is not configured.");
    }

    // OpenRouter est compatible avec l'API OpenAI
    var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
    {
        Endpoint = new Uri(baseUrl)
    });
    
    var chatClient = openAiClient.GetChatClient(modelName);
    
    return new ChatClientBuilder(chatClient.AsIChatClient())
        .UseFunctionInvocation()
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();