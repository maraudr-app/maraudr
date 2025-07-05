using Maraudr.MCP.Domain.Entities;
using Maraudr.MCP.Domain.ValueObjects;

namespace Maraudr.MCP.Domain.Interfaces;

public interface IMCPRepository
{
    Task<IEnumerable<McpTool>> GetAvailableToolsAsync();
    Task<ToolCallResult> CallToolAsync(string toolName, Dictionary<string, object> arguments);
    Task<bool> IsConnectedAsync();
    void SetUserJwt(string jwt);
    string GetUserJwt();
}