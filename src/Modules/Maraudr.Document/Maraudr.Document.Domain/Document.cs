namespace Maraudr.Document.Domain;

public class Document
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FileName { get; private set; }
    public string Url { get; private set; }
    public string ContentType { get; private set; }
    public Guid AssociationId { get; private set; }
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

    private Document() { }

    public Document(string fileName, string url, string contentType, Guid associationId)
    {
        FileName = fileName;
        Url = url;
        ContentType = contentType;
        AssociationId = associationId;
    }
}