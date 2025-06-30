using Maraudr.MCP.Infrastructure;
using MCP.Maraudr.Application;
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
    var apiKey = configuration["OpenAI:ApiKey"];
    var modelName = configuration["OpenAI:ModelName"];

    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(modelName))
    {
        throw new InvalidOperationException("OpenAI ApiKey or ModelName is not configured.");
    }

    var openAiClient = new OpenAIClient(apiKey);
    var chatClient = openAiClient.GetChatClient(modelName);
    
    return new ChatClientBuilder(chatClient as IChatClient ?? throw new InvalidOperationException("ChatClient does not implement IChatClient"))
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