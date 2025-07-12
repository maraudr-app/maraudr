using Maraudr.MCP.Infrastructure;
using MCP.Maraudr.Application;

using System.ClientModel;
using Maraudr.MCP.Endpoints;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

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

    var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
    {
        Endpoint = new Uri(baseUrl)
    });
    
    var chatClient = openAiClient.GetChatClient(modelName);
    
    return new ChatClientBuilder(chatClient.AsIChatClient())
        .UseFunctionInvocation()
        .Build();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://maraudr.eu", "https://www.maraudr.eu","https://maraudr-front-737l.onrender.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthenticationServicesForPlanning(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseRouting(); 
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.UseAuthentication();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.MapControllers();

app.Run();