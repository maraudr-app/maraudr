namespace Maraudr.Document.Application;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
}
