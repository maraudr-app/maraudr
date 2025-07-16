namespace MCPTests;

using Xunit;
using FluentAssertions;
using Maraudr.MCP.Domain.Entities;

    public class ConversationTests
    {
        [Fact]
        public void Constructor_ShouldCreateEmptyConversation()
        {
            // Act
            var conversation = new Conversation();

            // Assert
            conversation.Id.Should().NotBeEmpty();
            conversation.Messages.Should().BeEmpty();
            conversation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            conversation.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AddMessage_ShouldAddMessageAndUpdateTimestamp()
        {
            // Arrange
            var conversation = new Conversation();
            var message = new ChatMessage("user", "Hello");
            var initialLastUpdated = conversation.LastUpdated;

            // Act
            conversation.AddMessage(message);

            // Assert
            conversation.Messages.Should().HaveCount(1);
            conversation.Messages.First().Should().Be(message);
            conversation.LastUpdated.Should().BeAfter(initialLastUpdated);
        }
    }
