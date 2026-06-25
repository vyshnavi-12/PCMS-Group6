using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class CoverageGapAlertTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Physicians_UserId",
                table: "Physicians");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Physicians",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequestedByPhysicianId",
                table: "CoverageGapAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Physicians_UserId",
                table: "Physicians",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageGapAlerts_RequestedByPhysicianId",
                table: "CoverageGapAlerts",
                column: "RequestedByPhysicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoverageGapAlerts_Physicians_RequestedByPhysicianId",
                table: "CoverageGapAlerts",
                column: "RequestedByPhysicianId",
                principalTable: "Physicians",
                principalColumn: "PhysicianId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoverageGapAlerts_Physicians_RequestedByPhysicianId",
                table: "CoverageGapAlerts");

            migrationBuilder.DropIndex(
                name: "IX_Physicians_UserId",
                table: "Physicians");

            migrationBuilder.DropIndex(
                name: "IX_CoverageGapAlerts_RequestedByPhysicianId",
                table: "CoverageGapAlerts");

            migrationBuilder.DropColumn(
                name: "RequestedByPhysicianId",
                table: "CoverageGapAlerts");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Physicians",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Physicians_UserId",
                table: "Physicians",
                column: "UserId",
                unique: true);
        }
    }
}
