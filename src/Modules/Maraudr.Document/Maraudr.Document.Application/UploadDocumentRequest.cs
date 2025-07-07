using Microsoft.AspNetCore.Http;

namespace Maraudr.Document.Application;

public class UploadDocumentRequest
{
    public IFormFile File { get; set; } = default!;
}
