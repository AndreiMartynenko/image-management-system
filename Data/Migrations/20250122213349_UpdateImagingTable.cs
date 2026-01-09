using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareIMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImagingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagingType",
                table: "Imagings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiseaseType",
                table: "Imagings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_VisitId",
                table: "Invoices",
                column: "VisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Visits_VisitId",
                table: "Invoices",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Visits_VisitId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_VisitId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiseaseType",
                table: "Imagings");

            migrationBuilder.AlterColumn<string>(
                name: "ImagingType",
                table: "Imagings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
