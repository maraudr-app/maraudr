namespace Maraudr.Document.Domain;

public interface IDocumentRepository
{
    Task AddAsync(Document document);
    Task<IEnumerable<Document>> GetByAssociationAsync(Guid associationId);
}
