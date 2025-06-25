using Maraudr.Geo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Geo.Infrastructure;

public class GeoContext : DbContext
{
    public GeoContext(DbContextOptions<GeoContext> options) : base(options) { }

    public DbSet<Domain.Entities.GeoData> GeoEvents => Set<Domain.Entities.GeoData>();
    public DbSet<GeoStore> GeoStores => Set<GeoStore>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeoStore>()
            .HasMany(store => store.GeoEvents)
            .WithOne(data => data.GeoStore!)
            .HasForeignKey(data => data.GeoStoreId)
            .OnDelete(DeleteBehavior.Cascade);

        // Shadow property de type point
        modelBuilder.Entity<Domain.Entities.GeoData>()
            .Property<NetTopologySuite.Geometries.Point>("Location")
            .HasColumnType("geography (point)");

        base.OnModelCreating(modelBuilder);
    }


}