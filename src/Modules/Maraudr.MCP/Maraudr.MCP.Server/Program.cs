using Maraudr.MCP.Infrastructure;
using Maraudr.MCP.Server.Tools;
using MCP.Maraudr.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // Configurer ApiSettings
    services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));
    services.AddTransient<Tools>();
    services.AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();
    services.AddInfrastructure(context.Configuration);
    services.AddApplication(); 

});

var host = builder.Build();


await host.RunAsync();