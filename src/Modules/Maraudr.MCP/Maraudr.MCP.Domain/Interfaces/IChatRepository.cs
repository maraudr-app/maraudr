using Maraudr.MCP.Domain.Entities;

namespace Maraudr.MCP.Domain.Interfaces;

public interface IChatRepository
{
    Task<IAsyncEnumerable<string>> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages, 
        IEnumerable<McpTool> availableTools);
    Task<string> GetResponseAsync(
        IEnumerable<ChatMessage> messages, 
        IEnumerable<McpTool> availableTools);
}