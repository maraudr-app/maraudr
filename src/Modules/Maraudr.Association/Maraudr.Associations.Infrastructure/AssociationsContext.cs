using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Siret;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maraudr.Associations.Infrastructure;

public class AssociationsContext(DbContextOptions<AssociationsContext> options) : DbContext(options)
{
    public DbSet<Association> Associations { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Conversion et comparaison de SiretNumber
        var siretConverter = new ValueConverter<SiretNumber, string>(
            v => v.Value,
            v => new SiretNumber(v)
        );

        var siretComparer = new ValueComparer<SiretNumber>(
            (s1, s2) => s1!.Value == s2!.Value,
            s => s.Value.GetHashCode(),
            s => new SiretNumber(s.Value)
        );

        modelBuilder.Entity<Association>(builder =>
        {
            builder.ToTable("associations");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.City);
            builder.Property(a => a.Country);

            builder.Property(a => a.Members)
                .HasColumnType("uuid[]");

            builder.Property(a => a.Siret)
                .HasConversion(siretConverter)
                .HasColumnName("siret")
                .HasColumnType("text")
                .Metadata.SetValueComparer(siretComparer);

            builder.OwnsOne(a => a.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("street")
                    .IsRequired();

                address.Property(a => a.PostalCode)
                    .HasColumnName("postal_code")
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("address_city")
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("address_country")
                    .IsRequired();
            });
        });
    }
}
