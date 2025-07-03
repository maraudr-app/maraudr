using Maraudr.Document.Domain;

namespace Maraudr.Document.Application;

public class DocumentService(IDocumentRepository repository, IDocumentStorageService storage)
{
    public async Task UploadDocumentAsync(UploadDocumentRequest request, Guid associationId)
    {
        var url = await storage.UploadAsync(request.File, associationId);

        var document = new Domain.Document(
            request.File.FileName,
            url,
            request.File.ContentType,
            associationId
        );

        await repository.AddAsync(document);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsAsync(Guid associationId)
    {
        var documents = await repository.GetByAssociationAsync(associationId);
        return documents.Select(d => new DocumentDto
        {
            Id = d.Id,
            FileName = d.FileName,
            Url = d.Url,
            ContentType = d.ContentType,
            UploadedAt = d.UploadedAt
        });
    }
}
