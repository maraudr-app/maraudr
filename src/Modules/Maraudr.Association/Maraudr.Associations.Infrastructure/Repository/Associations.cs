using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Associations.Infrastructure.Repository;

public class AssocationsRepository(AssociationsContext context) : IAssociations
{
    public async Task<Association?> RegisterAssociation(Association? association)
    {
        await context.Associations.AddAsync(association);
        await context.SaveChangesAsync();
        return association;
    }

    public async Task UnregisterAssociation(Guid id)
    {
        var association = await context.Associations.FindAsync(id);
        if (association is not null)
        {
            context.Associations.Remove(association);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Association?> GetAssociation(Guid id)
    {
        var association = await context.Associations.FindAsync(id);
        return association;
    }
    
    public async Task<List<Association?>> ListPaginated(int skip, int take)
    {
        return await context.Associations
            .AsNoTracking()
            .OrderBy(a => a!.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Association?> UpdateAssociation(Association association)
    {
        var existing = await context.Associations.FindAsync(association.Id);
        if (existing is null) return null;

        existing.UpdateInformation(association.Name, association.Address);

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<Association?> GetAssociationBySiret(string siret)
    {
        var association = await context.Associations.Where(s => s!.Siret!.Value == siret).FirstOrDefaultAsync();
        return association;
    }
    
    public async Task<List<Association>> SearchAssociationsByName(string name)
    {
        return await context.Associations
            .Where(s => EF.Functions.Like(s.Name, $"%{name}%"))
            .ToListAsync();
    }
    
    public async Task<List<Association>> SearchAssociationsByCity(string city)
    {
        return await context.Associations
            .Where(a => EF.Functions.Like(a.City!, $"%{city}%"))
            .ToListAsync();
    }
}
