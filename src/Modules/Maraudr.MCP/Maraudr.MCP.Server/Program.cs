using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();
});

await builder.Build().RunAsync();