using FluentAssertions;
using Maraudr.MCP.Domain.Entities;

namespace MCPTests;

public class ChatTests
{
    
    public class ChatMessageTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var role = "user";
            var content = "Hello world";
            var toolCallId = "tool_123";

            // Act
            var message = new ChatMessage(role, content, toolCallId);

            // Assert
            message.Role.Should().Be(role);
            message.Content.Should().Be(content);
            message.ToolCallId.Should().Be(toolCallId);
            message.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}