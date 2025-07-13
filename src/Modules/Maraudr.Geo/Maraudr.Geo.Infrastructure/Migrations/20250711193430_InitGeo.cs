using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Maraudr.Geo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitGeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "GeoStores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssociationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoStores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Itineraries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssociationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartLat = table.Column<double>(type: "double precision", nullable: false),
                    StartLng = table.Column<double>(type: "double precision", nullable: false),
                    CenterLat = table.Column<double>(type: "double precision", nullable: false),
                    CenterLng = table.Column<double>(type: "double precision", nullable: false),
                    RadiusKm = table.Column<double>(type: "double precision", nullable: false),
                    GeoJson = table.Column<string>(type: "text", nullable: false),
                    GoogleMapsUrl = table.Column<string>(type: "text", nullable: false),
                    DistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    DurationMinutes = table.Column<double>(type: "double precision", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itineraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeoEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoStoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    ObservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<Point>(type: "geography (point)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoEvents_GeoStores_GeoStoreId",
                        column: x => x.GeoStoreId,
                        principalTable: "GeoStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeoEvents_GeoStoreId",
                table: "GeoEvents",
                column: "GeoStoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoEvents");

            migrationBuilder.DropTable(
                name: "Itineraries");

            migrationBuilder.DropTable(
                name: "GeoStores");
        }
    }
}
