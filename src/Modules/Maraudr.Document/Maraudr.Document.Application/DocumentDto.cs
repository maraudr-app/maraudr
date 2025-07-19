namespace Maraudr.Document.Application;

public record DocumentDto(
    Guid Id,
    string FileName,
    string Key,
    string ContentType,
    DateTime UploadedAt,
    string Url
);