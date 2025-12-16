using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddOrdonnancesToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ordonnance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PharmacienId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordonnance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordonnance_AspNetUsers_PharmacienId",
                        column: x => x.PharmacienId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordonnance_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicament",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdonnanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicament", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicament_Ordonnance_OrdonnanceId",
                        column: x => x.OrdonnanceId,
                        principalTable: "Ordonnance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicament_OrdonnanceId",
                table: "Medicament",
                column: "OrdonnanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordonnance_PatientId",
                table: "Ordonnance",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordonnance_PharmacienId",
                table: "Ordonnance",
                column: "PharmacienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicament");

            migrationBuilder.DropTable(
                name: "Ordonnance");

            migrationBuilder.DropTable(
                name: "Patient");
        }
    }
}
