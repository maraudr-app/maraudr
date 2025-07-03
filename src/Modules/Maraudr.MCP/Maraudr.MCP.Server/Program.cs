using Maraudr.MCP.Infrastructure;
using Maraudr.MCP.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Configurer ApiSettings
    services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));
    
    services.AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();
    services.AddInfrastructure(context.Configuration);
});

var host = builder.Build();

Tools.Configure(host.Services);

await host.RunAsync();