using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Domain.Siret;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Associations.Infrastructure.Repository;

public class AssocationsRepository(AssociationsContext context) : IAssociations
{
    public async Task<Association?> RegisterAssociation(Association? association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (association.Siret is null)
            throw new ArgumentException("SIRET is required.");

        var exists = await context.Associations
            .AnyAsync(a => a.Siret == association.Siret);

        if (exists)
            throw new InvalidOperationException("Une association avec ce SIRET existe déjà.");

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
        return await context.Associations.FindAsync(id);
    }

    public async Task AddUserToAssociationAsync(Guid associationId, Guid userId)
    {
        var association = await context.Associations.FindAsync(associationId);
        if (association == null)
            throw new ArgumentException("Association not found");

        if (association.Members.Contains(userId))
            throw new InvalidOperationException("User is already a member of this association");

        association.Members.Add(userId);
        await context.SaveChangesAsync();
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
        return await context.Associations
            .FirstOrDefaultAsync(a => a.Siret == new SiretNumber(siret));
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
    
    public async Task<IEnumerable<Guid>> GetAssociationIdsByUserIdAsync(Guid userId)
    {
        return await context.Associations
            .Where(a => a.Members.Contains(userId))
            .Select(a => a.Id)
            .ToListAsync();
    }
}
