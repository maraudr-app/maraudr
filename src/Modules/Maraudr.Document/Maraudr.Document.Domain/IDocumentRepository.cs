namespace Maraudr.Document.Domain;

public interface IDocumentRepository
{
    Task AddAsync(Document document);
    Task<IEnumerable<Document>> GetByAssociationAsync(Guid associationId);
    Task DeleteAsync(Document document);
    Task<Document?> GetByIdAsync(Guid id);
}
