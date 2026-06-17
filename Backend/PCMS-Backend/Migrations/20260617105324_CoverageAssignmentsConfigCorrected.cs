using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class CoverageAssignmentsConfigCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoverageAssignments_CoverageScheduleId_CoverageDate_SpecialtyId",
                table: "CoverageAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_CoverageScheduleId_CoverageDate_SpecialtyId_ShiftType",
                table: "CoverageAssignments",
                columns: new[] { "CoverageScheduleId", "CoverageDate", "SpecialtyId", "ShiftType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoverageAssignments_CoverageScheduleId_CoverageDate_SpecialtyId_ShiftType",
                table: "CoverageAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_CoverageScheduleId_CoverageDate_SpecialtyId",
                table: "CoverageAssignments",
                columns: new[] { "CoverageScheduleId", "CoverageDate", "SpecialtyId" },
                unique: true);
        }
    }
}
