using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maraudr.User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitUserDisponibilities3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilities_Users_UserId",
                table: "Disponibilities");

            migrationBuilder.DropIndex(
                name: "IX_Disponibilities_UserId",
                table: "Disponibilities");

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilities_AssociationId",
                table: "Disponibilities",
                column: "AssociationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilities_Users_AssociationId",
                table: "Disponibilities",
                column: "AssociationId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilities_Users_AssociationId",
                table: "Disponibilities");

            migrationBuilder.DropIndex(
                name: "IX_Disponibilities_AssociationId",
                table: "Disponibilities");

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilities_UserId",
                table: "Disponibilities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilities_Users_UserId",
                table: "Disponibilities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
