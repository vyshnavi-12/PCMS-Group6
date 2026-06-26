using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class updatedSwapRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwapRequests_CoverageAssignments_CoverageAssignmentId",
                table: "SwapRequests");

            migrationBuilder.RenameColumn(
                name: "CoverageAssignmentId",
                table: "SwapRequests",
                newName: "TargetedPhysicianCoverageAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_SwapRequests_CoverageAssignmentId",
                table: "SwapRequests",
                newName: "IX_SwapRequests_TargetedPhysicianCoverageAssignmentId");

            migrationBuilder.AddColumn<int>(
                name: "RequestedPhysicianCoverageAssignmentId",
                table: "SwapRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_RequestedPhysicianCoverageAssignmentId",
                table: "SwapRequests",
                column: "RequestedPhysicianCoverageAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SwapRequests_CoverageAssignments_RequestedPhysicianCoverageAssignmentId",
                table: "SwapRequests",
                column: "RequestedPhysicianCoverageAssignmentId",
                principalTable: "CoverageAssignments",
                principalColumn: "CoverageAssignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SwapRequests_CoverageAssignments_TargetedPhysicianCoverageAssignmentId",
                table: "SwapRequests",
                column: "TargetedPhysicianCoverageAssignmentId",
                principalTable: "CoverageAssignments",
                principalColumn: "CoverageAssignmentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwapRequests_CoverageAssignments_RequestedPhysicianCoverageAssignmentId",
                table: "SwapRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SwapRequests_CoverageAssignments_TargetedPhysicianCoverageAssignmentId",
                table: "SwapRequests");

            migrationBuilder.DropIndex(
                name: "IX_SwapRequests_RequestedPhysicianCoverageAssignmentId",
                table: "SwapRequests");

            migrationBuilder.DropColumn(
                name: "RequestedPhysicianCoverageAssignmentId",
                table: "SwapRequests");

            migrationBuilder.RenameColumn(
                name: "TargetedPhysicianCoverageAssignmentId",
                table: "SwapRequests",
                newName: "CoverageAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_SwapRequests_TargetedPhysicianCoverageAssignmentId",
                table: "SwapRequests",
                newName: "IX_SwapRequests_CoverageAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SwapRequests_CoverageAssignments_CoverageAssignmentId",
                table: "SwapRequests",
                column: "CoverageAssignmentId",
                principalTable: "CoverageAssignments",
                principalColumn: "CoverageAssignmentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
