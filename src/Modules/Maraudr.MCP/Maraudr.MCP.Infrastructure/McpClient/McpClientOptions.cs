namespace Maraudr.MCP.Infrastructure.McpClient;

public class McpClientOptions
{
    public const string SectionName = "McpClient";
        
    public string Command { get; set; } = "dotnet";
    public List<string>? Arguments { get; set; }
    public string Name { get; set; } = "MCP Server";
    public string? WorkingDirectory { get; set; }
}