using Microsoft.AspNetCore.Http;

namespace Maraudr.Document.Application;

public interface IDocumentStorageService
{
    Task<(string Url, string Key)> UploadAsync(IFormFile file, Guid associationId);
    Task DeleteAsync(string key);
    Task<string> GeneratePresignedUrlAsync(string key, TimeSpan validFor);

}
