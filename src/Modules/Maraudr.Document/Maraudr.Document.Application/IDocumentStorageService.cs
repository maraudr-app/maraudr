using Microsoft.AspNetCore.Http;

namespace Maraudr.Document.Application;

public interface IDocumentStorageService
{
    Task<string> UploadAsync(IFormFile file, Guid associationId);
}
