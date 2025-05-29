using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Siret;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maraudr.Associations.Infrastructure;

public class AssociationsContext(DbContextOptions<AssociationsContext> options) : DbContext(options)
{
    public DbSet<Association?> Associations { get; init; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<SiretNumber, string>(
            v => v.Value,
            v => new SiretNumber(v)
        );

        modelBuilder.Entity<Association>(builder =>
        {
            builder.Property(a => a.Siret)
                .HasConversion(converter);

            builder.OwnsOne(a => a.Address, address =>
            {
               
            });
        });
    }

}