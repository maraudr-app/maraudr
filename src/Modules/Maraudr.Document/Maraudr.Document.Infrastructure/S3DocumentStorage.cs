using Amazon.S3;
using Amazon.S3.Model;
using Maraudr.Document.Application;
using Microsoft.AspNetCore.Http;

namespace Maraudr.Document.Infrastructure;

public class S3DocumentStorageService(IAmazonS3 s3) : IDocumentStorageService
{
    private const string BucketName = "maraudr-files";

    public async Task<(string Url, string Key)> UploadAsync(IFormFile file, Guid associationId)
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

        var url = $"https://{BucketName}.s3.{s3.Config.RegionEndpoint.SystemName}.amazonaws.com/{Uri.EscapeDataString(key)}";
        return (url, key);
    }

    
    public async Task DeleteAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = BucketName,
            Key = key
        };

        await s3.DeleteObjectAsync(request);
    }
    
    public async Task<string> GeneratePresignedUrlAsync(string key, TimeSpan validFor)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = BucketName,
            Key = key,
            Expires = DateTime.UtcNow.Add(validFor),
            Verb = HttpVerb.GET
        };

        var url = await s3.GetPreSignedURLAsync(request);
        return await Task.FromResult(url);
    }
}
