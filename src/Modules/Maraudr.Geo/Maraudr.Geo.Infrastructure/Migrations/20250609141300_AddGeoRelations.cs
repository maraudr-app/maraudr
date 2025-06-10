using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maraudr.Geo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeoRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "GeoEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoStoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    ObservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
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
                name: "GeoStores");
        }
    }
}
