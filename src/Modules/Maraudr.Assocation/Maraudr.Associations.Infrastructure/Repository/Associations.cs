using Maraudr.Associations.Domain.Entities;
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

    public async Task<Association?> UpdateAssociation(Association association)
    {
        var asso = await context.Associations.FindAsync(association.Id);
        if (asso is null) return null;
        context.Entry(asso).CurrentValues.SetValues(association);
        await context.SaveChangesAsync();
        return asso;
    }

    public async Task<Association?> GetAssociationBySiret(string siret)
    {
        var association = await context.Associations.Where(s => s!.Siret!.Value == siret).FirstOrDefaultAsync();
        return association;
    }
}
