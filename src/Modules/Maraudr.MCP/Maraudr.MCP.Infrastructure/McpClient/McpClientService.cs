using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using IMcpClient = ModelContextProtocol.Client.IMcpClient;
using McpClientFactory = ModelContextProtocol.Client.McpClientFactory;

namespace Maraudr.MCP.Infrastructure.McpClient;

public class McpClientService
{
    private static IMcpClient? _mcpClient;
    private static readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private readonly McpClientOptions _options;
    private readonly ILoggerFactory _loggerFactory;

    public McpClientService(
        IOptions<McpClientOptions> options,
        ILoggerFactory loggerFactory)
    {
        _options = options.Value;
        _loggerFactory = loggerFactory;
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

            var transportOptions = new StdioClientTransportOptions
            {
                Command = _options.Command,
                Arguments = _options.Arguments,
                WorkingDirectory = _options.WorkingDirectory
            };

            var transport = new StdioClientTransport(transportOptions, _loggerFactory);
            
            _mcpClient = await McpClientFactory.CreateAsync(transport);

            return _mcpClient;
        }
        finally
        {
            _initSemaphore.Release();
        }
    }
}