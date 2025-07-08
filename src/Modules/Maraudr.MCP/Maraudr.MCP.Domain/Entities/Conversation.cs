namespace Maraudr.MCP.Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; }
    public List<ChatMessage> Messages { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public Conversation()
    {
        Id = Guid.NewGuid();
        Messages = new List<ChatMessage>();
        CreatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
    }

    public void AddMessage(ChatMessage message)
    {
        Messages.Add(message);
        LastUpdated = DateTime.UtcNow;
    }

    public void AddMessages(IEnumerable<ChatMessage> messages)
    {
        Messages.AddRange(messages);
        LastUpdated = DateTime.UtcNow;
    }
}