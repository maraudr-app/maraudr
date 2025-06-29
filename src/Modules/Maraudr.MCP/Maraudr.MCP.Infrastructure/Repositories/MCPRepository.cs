using Maraudr.MCP.Domain.Entities;
using Maraudr.MCP.Domain.Interfaces;
using Maraudr.MCP.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class MCPRepository(IConfiguration configuration) : IMCPRepository
{
        private static IMcpClient? _mcpClient;
        private static readonly SemaphoreSlim _initSemaphore = new(1, 1);

        public async Task<IEnumerable<McpTool>> GetAvailableToolsAsync()
        {
            var client = await GetMcpClientAsync();
            var tools = await client.ListToolsAsync();
            
            return tools.Select(t => new McpTool(
                t.Name,
                t.Description ?? "",
                new Dictionary<string, object>() 
            ));
        }

        public async Task<ToolCallResult> CallToolAsync(string toolName, Dictionary<string, object> arguments)
        {
            try
            {
                var client = await GetMcpClientAsync();
                var result = await client.CallToolAsync(toolName, arguments);
                
                return new ToolCallResult(toolName, arguments, result, true);
            }
            catch (Exception ex)
            {
                return new ToolCallResult(toolName, arguments, null!, false, ex.Message);
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                var client = await GetMcpClientAsync();
                // You might want to add a ping method to verify connection
                return client != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task<IMcpClient> GetMcpClientAsync()
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
                        Command = "dotnet",
                        Arguments = ["run", "--project", configuration["McpServer:ProjectPath"] ?? "../McpServer"],
                        Name = "MCP Server",
                    }));
                    
                return _mcpClient;
            }
            finally
            {
                _initSemaphore.Release();
            }
        }
    }