using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddQuantiteToOrdonnanceMedicament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "OrdonnanceMedicaments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "OrdonnanceMedicaments");
        }
    }
}
