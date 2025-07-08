namespace Maraudr.MCP.Domain.Entities;


    public class ChatMessage
    {
        public string Role { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? ToolCallId { get; private set; }

        public ChatMessage(string role, string content, string? toolCallId = null)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Timestamp = DateTime.UtcNow;
            ToolCallId = toolCallId;
        }
    }

