namespace Maraudr.Document.Domain;

public class Document
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FileName { get; private set; }
    public string Key { get; private set; }
    public string Url { get; private set; } 
    public string ContentType { get; private set; }
    public Guid AssociationId { get; private set; }
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

    private Document() { }

    public Document(string fileName, string key, string url, string contentType, Guid associationId)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be null or empty");
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be null or empty");
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("Url cannot be null or empty");
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("ContentType cannot be null or empty");
    
        FileName = fileName;
        Key = key;
        Url = url;
        ContentType = contentType;
        AssociationId = associationId;
    }
}