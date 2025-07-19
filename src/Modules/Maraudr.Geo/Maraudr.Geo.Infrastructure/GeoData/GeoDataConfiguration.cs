using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Maraudr.Geo.Domain.Entities;
using Point = NetTopologySuite.Geometries.Point;

public class GeoDataConfiguration : IEntityTypeConfiguration<GeoData>
{
    public void Configure(EntityTypeBuilder<GeoData> builder)
    {
        builder.Property<double>(e => e.Latitude);
        builder.Property<double>(e => e.Longitude);
        builder.Property<DateTime>(e => e.ObservedAt);
        builder.Property<string?>("Notes");

        builder.Property<Point>("Location")
            .HasColumnType("geography (point)");
    }
}