using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class CoverageGapTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoverageGapAlerts_Physicians_SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts");

            migrationBuilder.DropIndex(
                name: "IX_CoverageGapAlerts_SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts");

            migrationBuilder.DropColumn(
                name: "SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoverageGapAlerts_SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts",
                column: "SuggestedReplacementPhysicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoverageGapAlerts_Physicians_SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts",
                column: "SuggestedReplacementPhysicianId",
                principalTable: "Physicians",
                principalColumn: "PhysicianId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
