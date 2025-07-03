using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maraudr.Geo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItinerariesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssociationId",
                table: "Itineraries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "CenterLat",
                table: "Itineraries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CenterLng",
                table: "Itineraries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Itineraries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "RadiusKm",
                table: "Itineraries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartLat",
                table: "Itineraries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartLng",
                table: "Itineraries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssociationId",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "CenterLat",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "CenterLng",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "RadiusKm",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "StartLat",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "StartLng",
                table: "Itineraries");
        }
    }
}
