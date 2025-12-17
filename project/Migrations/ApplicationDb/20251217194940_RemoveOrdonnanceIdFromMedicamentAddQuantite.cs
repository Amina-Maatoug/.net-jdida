using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class RemoveOrdonnanceIdFromMedicamentAddQuantite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint first
            migrationBuilder.DropForeignKey(
                name: "FK_Medicaments_Ordonnances_OrdonnanceId",
                table: "Medicaments");

            // Drop the index
            migrationBuilder.DropIndex(
                name: "IX_Medicaments_OrdonnanceId",
                table: "Medicaments");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "OrdonnanceId",
                table: "Medicaments");

            // Add the new column
            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "Medicaments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Create the junction table
            migrationBuilder.CreateTable(
                name: "OrdonnanceMedicaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdonnanceId = table.Column<int>(type: "int", nullable: false),
                    MedicamentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdonnanceMedicaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdonnanceMedicaments_Medicaments_MedicamentId",
                        column: x => x.MedicamentId,
                        principalTable: "Medicaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdonnanceMedicaments_Ordonnances_OrdonnanceId",
                        column: x => x.OrdonnanceId,
                        principalTable: "Ordonnances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdonnanceMedicaments_MedicamentId",
                table: "OrdonnanceMedicaments",
                column: "MedicamentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdonnanceMedicaments_OrdonnanceId",
                table: "OrdonnanceMedicaments",
                column: "OrdonnanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdonnanceMedicaments");

            migrationBuilder.DropTable(
                name: "Medicaments");

            migrationBuilder.DropTable(
                name: "Ordonnances");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
