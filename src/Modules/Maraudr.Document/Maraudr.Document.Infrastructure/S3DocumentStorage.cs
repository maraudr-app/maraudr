using Amazon.S3;
using Amazon.S3.Model;
using Maraudr.Document.Application;
using Microsoft.AspNetCore.Http;

namespace Maraudr.Document.Infrastructure;

public class S3DocumentStorageService(IAmazonS3 s3) : IDocumentStorageService
{
    private const string BucketName = "maraudr-files";

    public async Task<string> UploadAsync(IFormFile file, Guid associationId)
    {
        var key = $"{associationId}/{Guid.NewGuid()}_{file.FileName}";

        await using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType,
            AutoCloseStream = true
        };

        await s3.PutObjectAsync(request);

        return $"https://{BucketName}.s3.amazonaws.com/{key}";
    }
}
