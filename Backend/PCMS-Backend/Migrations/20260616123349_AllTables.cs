using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PCMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    SpecialtyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialtyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.SpecialtyId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityRecordId = table.Column<int>(type: "int", nullable: false),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoverageSchedules",
                columns: table => new
                {
                    CoverageScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WeekStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    WeekEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PublishedByUserId = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageSchedules", x => x.CoverageScheduleId);
                    table.ForeignKey(
                        name: "FK_CoverageSchedules_Users_PublishedByUserId",
                        column: x => x.PublishedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NotificationTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NotificationMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Physicians",
                columns: table => new
                {
                    PhysicianId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PhysicianCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MedicalLicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmploymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Physicians", x => x.PhysicianId);
                    table.ForeignKey(
                        name: "FK_Physicians_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoverageAssignments",
                columns: table => new
                {
                    CoverageAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoverageScheduleId = table.Column<int>(type: "int", nullable: false),
                    CoverageDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    PhysicianId = table.Column<int>(type: "int", nullable: false),
                    ShiftType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageAssignments", x => x.CoverageAssignmentId);
                    table.ForeignKey(
                        name: "FK_CoverageAssignments_CoverageSchedules_CoverageScheduleId",
                        column: x => x.CoverageScheduleId,
                        principalTable: "CoverageSchedules",
                        principalColumn: "CoverageScheduleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoverageAssignments_Physicians_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoverageAssignments_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalLeavesData",
                columns: table => new
                {
                    ExternalLeaveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianId = table.Column<int>(type: "int", nullable: false),
                    LeaveStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LeaveEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LeaveReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalLeavesData", x => x.ExternalLeaveId);
                    table.ForeignKey(
                        name: "FK_ExternalLeavesData_Physicians_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalShiftsData",
                columns: table => new
                {
                    ExternalShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianId = table.Column<int>(type: "int", nullable: false),
                    ShiftDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalShiftsData", x => x.ExternalShiftId);
                    table.ForeignKey(
                        name: "FK_ExternalShiftsData_Physicians_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysicianSpecialtyMaps",
                columns: table => new
                {
                    PhysicianSpecialtyMapId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianId = table.Column<int>(type: "int", nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    IsPrimarySpecialty = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicianSpecialtyMaps", x => x.PhysicianSpecialtyMapId);
                    table.ForeignKey(
                        name: "FK_PhysicianSpecialtyMaps_Physicians_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhysicianSpecialtyMaps_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoverageGapAlerts",
                columns: table => new
                {
                    CoverageGapAlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoverageAssignmentId = table.Column<int>(type: "int", nullable: false),
                    SuggestedReplacementPhysicianId = table.Column<int>(type: "int", nullable: true),
                    AlertStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AlertReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageGapAlerts", x => x.CoverageGapAlertId);
                    table.ForeignKey(
                        name: "FK_CoverageGapAlerts_CoverageAssignments_CoverageAssignmentId",
                        column: x => x.CoverageAssignmentId,
                        principalTable: "CoverageAssignments",
                        principalColumn: "CoverageAssignmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoverageGapAlerts_Physicians_SuggestedReplacementPhysicianId",
                        column: x => x.SuggestedReplacementPhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SwapRequests",
                columns: table => new
                {
                    SwapRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoverageAssignmentId = table.Column<int>(type: "int", nullable: false),
                    RequestedByPhysicianId = table.Column<int>(type: "int", nullable: false),
                    TargetPhysicianId = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestComments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByUserId = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwapRequests", x => x.SwapRequestId);
                    table.ForeignKey(
                        name: "FK_SwapRequests_CoverageAssignments_CoverageAssignmentId",
                        column: x => x.CoverageAssignmentId,
                        principalTable: "CoverageAssignments",
                        principalColumn: "CoverageAssignmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SwapRequests_Physicians_RequestedByPhysicianId",
                        column: x => x.RequestedByPhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SwapRequests_Physicians_TargetPhysicianId",
                        column: x => x.TargetPhysicianId,
                        principalTable: "Physicians",
                        principalColumn: "PhysicianId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SwapRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "House Supervisor" },
                    { 2, "Physician" }
                });

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "SpecialtyId", "IsActive", "SpecialtyName" },
                values: new object[,]
                {
                    { 1, true, "Cardiology" },
                    { 2, true, "Neurology" },
                    { 3, true, "Orthopedics" },
                    { 4, true, "Emergency Medicine" },
                    { 5, true, "Radiology" },
                    { 6, true, "Anesthesiology" },
                    { 7, true, "General Surgery" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_CoverageScheduleId_CoverageDate_SpecialtyId",
                table: "CoverageAssignments",
                columns: new[] { "CoverageScheduleId", "CoverageDate", "SpecialtyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_PhysicianId",
                table: "CoverageAssignments",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAssignments_SpecialtyId",
                table: "CoverageAssignments",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageGapAlerts_CoverageAssignmentId",
                table: "CoverageGapAlerts",
                column: "CoverageAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageGapAlerts_SuggestedReplacementPhysicianId",
                table: "CoverageGapAlerts",
                column: "SuggestedReplacementPhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageSchedules_PublishedByUserId",
                table: "CoverageSchedules",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLeavesData_PhysicianId",
                table: "ExternalLeavesData",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalShiftsData_PhysicianId",
                table: "ExternalShiftsData",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Physicians_MedicalLicenseNumber",
                table: "Physicians",
                column: "MedicalLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Physicians_PhysicianCode",
                table: "Physicians",
                column: "PhysicianCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Physicians_UserId",
                table: "Physicians",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicianSpecialtyMaps_PhysicianId_SpecialtyId",
                table: "PhysicianSpecialtyMaps",
                columns: new[] { "PhysicianId", "SpecialtyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicianSpecialtyMaps_SpecialtyId",
                table: "PhysicianSpecialtyMaps",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_SpecialtyName",
                table: "Specialties",
                column: "SpecialtyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_CoverageAssignmentId",
                table: "SwapRequests",
                column: "CoverageAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_RequestedByPhysicianId",
                table: "SwapRequests",
                column: "RequestedByPhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_ReviewedByUserId",
                table: "SwapRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_TargetPhysicianId",
                table: "SwapRequests",
                column: "TargetPhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                table: "Users",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeCode",
                table: "Users",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CoverageGapAlerts");

            migrationBuilder.DropTable(
                name: "ExternalLeavesData");

            migrationBuilder.DropTable(
                name: "ExternalShiftsData");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PhysicianSpecialtyMaps");

            migrationBuilder.DropTable(
                name: "SwapRequests");

            migrationBuilder.DropTable(
                name: "CoverageAssignments");

            migrationBuilder.DropTable(
                name: "CoverageSchedules");

            migrationBuilder.DropTable(
                name: "Physicians");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
