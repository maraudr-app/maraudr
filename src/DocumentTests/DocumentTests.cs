using FluentAssertions;
using Maraudr.Document.Domain;

namespace DocumentTests;

public class DocumentTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        var fileName = "test.pdf";
        var key = "documents/test.pdf";
        var url = "https://example.com/test.pdf";
        var contentType = "application/pdf";
        var associationId = Guid.NewGuid();

        var document = new Document(fileName, key, url, contentType, associationId);

        document.FileName.Should().Be(fileName);
        document.Key.Should().Be(key);
        document.Url.Should().Be(url);
        document.ContentType.Should().Be(contentType);
        document.AssociationId.Should().Be(associationId);
        document.Id.Should().NotBeEmpty();
        document.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidFileName_ShouldThrowArgumentException(string invalidFileName)
    {
        // Arrange
        var key = "documents/test.pdf";
        var url = "https://example.com/test.pdf";
        var contentType = "application/pdf";
        var associationId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Document(invalidFileName, key, url, contentType, associationId));
    }
}