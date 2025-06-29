using Maraudr.MCP.Domain.Entities;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ChatMessage = Maraudr.MCP.Domain.Entities.ChatMessage;

namespace Maraudr.MCP.Infrastructure.Repositories;

 public class ChatRepository : IChatRepository
    {
        private readonly IChatClient _chatClient;

        public ChatRepository(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<string> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<McpTool> availableTools)
        {
            var chatMessages = ConvertToChatMessages(messages);
            var tools = ConvertToMcpClientTools(availableTools);
            
            var updates = new List<ChatResponseUpdate>();
            await foreach (var update in _chatClient.GetStreamingResponseAsync(chatMessages, new() { Tools = [.. tools] }))
            {
                updates.Add(update);
            }
            
            return string.Join("", updates.Where(u => !string.IsNullOrEmpty(u.Text)).Select(u => u.Text));
        }

        public async Task<IAsyncEnumerable<string>> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<McpTool> availableTools)
        {
            var chatMessages = ConvertToChatMessages(messages);
            var tools = ConvertToMcpClientTools(availableTools);
            
            return GetStreamingResponseInternal(chatMessages, tools);
        }

        private async IAsyncEnumerable<string> GetStreamingResponseInternal(
            List<Microsoft.Extensions.AI.ChatMessage> chatMessages, 
            List<McpClientTool> tools)
        {
            await foreach (var update in _chatClient.GetStreamingResponseAsync(chatMessages, new() { Tools = [.. tools] }))
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

        private List<McpClientTool> ConvertToMcpClientTools(IEnumerable<McpTool> tools)
        {
            // This would need to be implemented based on your actual McpClientTool structure
            // For now, returning empty list as placeholder
            return new List<McpClientTool>();
        }
    }