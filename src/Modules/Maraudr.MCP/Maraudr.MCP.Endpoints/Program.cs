// McpApi.Presentation/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddMcpClient(builder.Configuration);

// Add Chat Client
builder.Services.AddSingleton<IChatClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new ChatClientBuilder(
            new AzureOpenAIClient(
                    new Uri(configuration["AzureOpenAI:Endpoint"]!),
                    new DefaultAzureCredential())
                .GetChatClient(configuration["AzureOpenAI:ModelName"]!)
                .AsIChatClient())
        .UseFunctionInvocation()
        .Build();
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();