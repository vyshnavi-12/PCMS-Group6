using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedNamingConventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoverageAssignments_PhysicianId",
                table: "CoverageAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageGapAlerts_AlertStatus",
                table: "CoverageGapAlerts",
                column: "AlertStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_PhysicianId_CoverageDate",
                table: "CoverageAssignments",
                columns: new[] { "PhysicianId", "CoverageDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoverageGapAlerts_AlertStatus",
                table: "CoverageGapAlerts");

            migrationBuilder.DropIndex(
                name: "IX_CoverageAssignments_PhysicianId_CoverageDate",
                table: "CoverageAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_PhysicianId",
                table: "CoverageAssignments",
                column: "PhysicianId");
        }
    }
}
