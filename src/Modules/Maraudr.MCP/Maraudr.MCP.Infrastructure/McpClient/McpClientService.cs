using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using IMcpClient = McpToolkit.Client.IMcpClient;
using StdioClientTransport = McpToolkit.Client.StdioClientTransport;

namespace Maraudr.MCP.Infrastructure.McpClient;

public class McpClientService
{
    private static IMcpClient? _mcpClient;
    private static readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private readonly McpClientOptions _options;

    public McpClientService(
        IOptions<McpClientOptions> options,
        ILogger<McpClientService> logger)
    {
        _options = options.Value;
    }

    public async Task<IMcpClient> GetClientAsync()
    {
        if (_mcpClient != null)
            return _mcpClient;

        await _initSemaphore.WaitAsync();
        try
        {
            if (_mcpClient != null)
                return _mcpClient;

                
            _mcpClient = await McpClientFactory.CreateAsync(
                new StdioClientTransport(new()
                {
                    Command = _options.Command,
                    Arguments = _options.Arguments?.ToArray() ?? Array.Empty<string>(),
                    Name = _options.Name,
                    WorkingDirectory = _options.WorkingDirectory
                }));
                    
            return _mcpClient;
        }
        finally
        {
            _initSemaphore.Release();
        }
    }
}