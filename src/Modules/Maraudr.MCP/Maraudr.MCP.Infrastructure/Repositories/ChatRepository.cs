using Maraudr.MCP.Domain.Entities;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ChatMessage = Maraudr.MCP.Domain.Entities.ChatMessage;
using System.Text.Json;
using Maraudr.MCP.Infrastructure.McpClient;
using ModelContextProtocol.Client;
namespace Maraudr.MCP.Infrastructure.Repositories;

public class ChatRepository(IChatClient chatClient,McpClientService mcpClientService) : IChatRepository
{
    public async Task<string> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<McpTool> availableTools)
    {
        var chatMessages = ConvertToChatMessages(messages);

        var client =await  mcpClientService.GetClientAsync();
        var tools = await ConvertToMcpClientToolsAsync(availableTools, client);

        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, new() { Tools = [.. tools] }))
        {
            updates.Add(update);
        }

        return string.Join("", updates.Where(u => !string.IsNullOrEmpty(u.Text)).Select(u => u.Text));
    }

    public async Task<IAsyncEnumerable<string>> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<McpTool> availableTools)
    {
        var chatMessages = ConvertToChatMessages(messages);

        // Obtenez une instance de IMcpClient
        var client = await mcpClientService.GetClientAsync();/* Obtenez l'instance de IMcpClient */;
        var tools = await ConvertToMcpClientToolsAsync(availableTools, client);

        return GetStreamingResponseInternal(chatMessages, tools);
    }

    private async IAsyncEnumerable<string> GetStreamingResponseInternal(
        List<Microsoft.Extensions.AI.ChatMessage> chatMessages,
        List<McpClientTool> tools)
    {
        await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, new() { Tools = [.. tools] }))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }

    private List<Microsoft.Extensions.AI.ChatMessage> ConvertToChatMessages(IEnumerable<ChatMessage> messages)
    {
        return messages.Select(m => new Microsoft.Extensions.AI.ChatMessage(
            m.Role.ToLowerInvariant() switch
            {
                "user" => ChatRole.User,
                "assistant" => ChatRole.Assistant,
                "system" => ChatRole.System,
                _ => ChatRole.User
            },
            m.Content
        )).ToList();
    }

    private async Task<List<McpClientTool>> GetMcpClientToolsAsync(
        IMcpClient client, 
        JsonSerializerOptions? serializerOptions = null, 
        CancellationToken cancellationToken = default)
    {
        // Récupère les outils via le client MCP
        var tools = await client.ListToolsAsync(serializerOptions, cancellationToken);
        return tools.ToList();
    }

    private async Task<List<McpClientTool>> ConvertToMcpClientToolsAsync(
        IEnumerable<McpTool> tools, 
        IMcpClient client, 
        JsonSerializerOptions? serializerOptions = null, 
        CancellationToken cancellationToken = default)
    {
        // Appelle la méthode pour récupérer les outils
        return await GetMcpClientToolsAsync(client, serializerOptions, cancellationToken);
    }

}