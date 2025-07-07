using Maraudr.Document.Domain;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Document.Infrastructure;

public class DocumentRepository(DocumentContext context) : IDocumentRepository
{
    public async Task AddAsync(Domain.Document document)
    {
        context.Documents.Add(document);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Domain.Document>> GetByAssociationAsync(Guid associationId)
    {
        return await context.Documents
            .Where(d => d.AssociationId == associationId)
            .ToListAsync();
    }
    
    public async Task<Domain.Document?> GetByIdAsync(Guid id)
    {
        return await context.Documents.FindAsync(id);
    }
    
    public async Task DeleteAsync(Domain.Document document)
    {
        context.Documents.Remove(document);
        await context.SaveChangesAsync();
    }
}
