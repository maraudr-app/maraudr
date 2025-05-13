using Maraudr.Associations.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Associations.Infrastructure;

public class AssociationsContext(DbContextOptions<AssociationsContext> options) : DbContext(options)
{
    public DbSet<Association> Associations { get; init; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Association>().OwnsOne(a => a.Siret);
    }
}