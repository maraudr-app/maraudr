namespace Maraudr.MCP.Domain.Entities;

public class McpTool
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Dictionary<string, object> Schema { get; private set; }

    public McpTool(string name, string description, Dictionary<string, object>? schema = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Schema = schema ?? new Dictionary<string, object>();
    }
}