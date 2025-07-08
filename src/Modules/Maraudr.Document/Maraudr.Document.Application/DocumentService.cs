using Maraudr.Document.Domain;

namespace Maraudr.Document.Application;

public class DocumentService(IDocumentRepository repository, IDocumentStorageService storage)
{
    public async Task UploadDocumentAsync(UploadDocumentRequest request, Guid associationId)
    {
        var (url, key) = await storage.UploadAsync(request.File, associationId);

        var document = new Domain.Document(
            fileName: request.File.FileName,
            key: key,
            url: url,
            contentType: request.File.ContentType,
            associationId: associationId
        );

        await repository.AddAsync(document);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsAsync(Guid associationId)
    {
        var documents = await repository.GetByAssociationAsync(associationId);

        var result = await Task.WhenAll(documents.Select(async d =>
        {
            var signedUrl = await storage.GeneratePresignedUrlAsync(d.Key, TimeSpan.FromMinutes(10));
            return new DocumentDto(
                d.Id,
                d.FileName,
                d.Key,
                d.ContentType,
                d.UploadedAt,
                signedUrl
            );
        }));

        return result;
    }

    
    public async Task DeleteDocumentAsync(Guid documentId, Guid associationId)
    {
        var document = await repository.GetByIdAsync(documentId);
        if (document == null || document.AssociationId != associationId)
        {
            throw new KeyNotFoundException("Document not found or unauthorized.");
        }

        var bucketKey = document.Key;

        await storage.DeleteAsync(bucketKey);
        await repository.DeleteAsync(document);
    }

}
