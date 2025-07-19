using Maraudr.Geo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Maraudr.Geo.Infrastructure;

public class GeoContext : DbContext
{
    public GeoContext(DbContextOptions<GeoContext> options) : base(options) { }

    public DbSet<Domain.Entities.GeoData> GeoEvents => Set<Domain.Entities.GeoData>();
    public DbSet<GeoStore> GeoStores => Set<GeoStore>();
    public DbSet<Itinerary> Itineraries => Set<Itinerary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeoStore>()
            .HasMany(store => store.GeoEvents)
            .WithOne(data => data.GeoStore!)
            .HasForeignKey(data => data.GeoStoreId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Domain.Entities.GeoData>()
            .Property<Point>("Location")
            .HasColumnType("geography (point)");

        modelBuilder.Entity<Itinerary>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Itinerary>()
            .Property(i => i.GeoJson).HasColumnType("text");

        base.OnModelCreating(modelBuilder);
    }

}