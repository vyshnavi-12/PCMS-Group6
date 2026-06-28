using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Models;

namespace PCMS_Backend.Data;

public class PcmsDbContext : DbContext
{
    public PcmsDbContext(DbContextOptions<PcmsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Physician> Physicians { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<PhysicianSpecialtyMap> PhysicianSpecialtyMaps { get; set; }
    public DbSet<ExternalShiftsData> ExternalShiftsData { get; set; }
    public DbSet<ExternalLeavesData> ExternalLeavesData { get; set; }
    public DbSet<CoverageSchedule> CoverageSchedules { get; set; }
    public DbSet<CoverageAssignment> CoverageAssignments { get; set; }
    public DbSet<CoverageGapAlert> CoverageGapAlerts { get; set; }
    public DbSet<SwapRequest> SwapRequests { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public object UnavailableRequests { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureRole(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigurePhysician(modelBuilder);
        ConfigureSpecialty(modelBuilder);
        ConfigurePhysicianSpecialtyMap(modelBuilder);
        ConfigureCoverageSchedule(modelBuilder);
        ConfigureCoverageAssignment(modelBuilder);
        ConfigureCoverageGapAlert(modelBuilder);
        ConfigureSwapRequest(modelBuilder);
        ConfigureExternalShiftsData(modelBuilder);
        ConfigureExternalLeavesData(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureAuditLog(modelBuilder);

        // Seed Data
        SeedRoles(modelBuilder);
        SeedSpecialties(modelBuilder);
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.RoleId);

            entity.Property(r => r.RoleName)
                .HasMaxLength(50);

            entity.HasIndex(r => r.RoleName)
                .IsUnique();

            entity.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.EmployeeCode)
                .HasMaxLength(20);

            entity.Property(u => u.FullName)
                .HasMaxLength(100);

            entity.Property(u => u.EmailAddress)
                .HasMaxLength(150);

            entity.Property(u => u.PhoneNumber)
                .HasMaxLength(15);

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(u => u.EmployeeCode)
                .IsUnique();

            entity.HasIndex(u => u.EmailAddress)
                .IsUnique();

            entity.HasOne(u => u.Physician)
                .WithOne(p => p.User)
                .HasForeignKey<Physician>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.AuditLogs)
                .WithOne(a => a.PerformedByUser)
                .HasForeignKey(a => a.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.PublishedSchedules)
                .WithOne(cs => cs.PublishedByUser)
                .HasForeignKey(cs => cs.PublishedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.ReviewedSwapRequests)
                  .WithOne(sr => sr.ReviewedByUser)
                  .HasForeignKey (sr => sr.ReviewedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurePhysician(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(p => p.PhysicianId);

            entity.Property(p => p.PhysicianCode)
                .HasMaxLength(50);

            entity.Property(p => p.MedicalLicenseNumber)
                .HasMaxLength(100);

            entity.Property(p => p.EmploymentStatus)
                .HasMaxLength(50);

            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(p => p.UserId)
                .IsUnique();

            entity.HasIndex(p => p.PhysicianCode)
                .IsUnique();

            entity.HasIndex(p => p.MedicalLicenseNumber)
                .IsUnique();

            entity.HasMany(p => p.PhysicianSpecialtyMaps)
                .WithOne(psm => psm.Physician)
                .HasForeignKey(psm => psm.PhysicianId);

            entity.HasMany(p => p.ExternalShiftsData)
                .WithOne(esd => esd.Physician)
                .HasForeignKey(esd => esd.PhysicianId);

            entity.HasMany(p => p.ExternalLeavesData)
                .WithOne(eld => eld.Physician)
                .HasForeignKey(eld => eld.PhysicianId);

            entity.HasMany(p => p.CoverageAssignments)
                .WithOne(ca => ca.Physician)
                .HasForeignKey(ca => ca.PhysicianId)
                .OnDelete(DeleteBehavior.Restrict);

           

            entity.HasMany(p => p.RequestedSwapRequests)
                .WithOne(sr => sr.RequestedByPhysician)
                .HasForeignKey(sr => sr.RequestedByPhysicianId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.TargetSwapRequests)
                .WithOne(sr => sr.TargetPhysician)
                .HasForeignKey(sr => sr.TargetPhysicianId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureSpecialty(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(s => s.SpecialtyId);

            entity.Property(s => s.SpecialtyName)
                .HasMaxLength(100);

            entity.Property(s => s.IsActive)
                .HasDefaultValue(true);

            entity.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(s => s.SpecialtyName)
                .IsUnique();

            entity.HasMany(s => s.PhysicianSpecialtyMaps)
                .WithOne(psm => psm.Specialty)
                .HasForeignKey(psm => psm.SpecialtyId);

            entity.HasMany(s => s.CoverageAssignments)
                .WithOne(ca => ca.Specialty)
                .HasForeignKey(ca => ca.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurePhysicianSpecialtyMap(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhysicianSpecialtyMap>(entity =>
        {
            entity.HasKey(psm => psm.PhysicianSpecialtyMapId);

            entity.Property(psm => psm.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(psm => new
            {
                psm.PhysicianId,
                psm.SpecialtyId
            })
            .IsUnique();
        });
    }

    private static void ConfigureCoverageSchedule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoverageSchedule>(entity =>
        {
            entity.HasKey(cs => cs.CoverageScheduleId);

            entity.Property(cs => cs.ScheduleName)
                .HasMaxLength(200);

            entity.Property(cs => cs.Status)
                .HasMaxLength(50);

            entity.Property(cs => cs.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(cs => cs.CoverageAssignments)
                .WithOne(ca => ca.CoverageSchedule)
                .HasForeignKey(ca => ca.CoverageScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCoverageAssignment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoverageAssignment>(entity =>
        {
            entity.HasKey(ca => ca.CoverageAssignmentId);

            entity.Property(ca => ca.ShiftType)
                .HasMaxLength(50);

            entity.Property(ca => ca.AssignmentStatus)
                .HasMaxLength(50);

            entity.Property(ca => ca.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(ca => new
            {
                ca.CoverageScheduleId,
                ca.CoverageDate,
                ca.SpecialtyId,
                ca.ShiftType
            })
            .IsUnique();

            entity.HasMany(ca => ca.CoverageGapAlerts)
                .WithOne(cga => cga.CoverageAssignment)
                .HasForeignKey(cga => cga.CoverageAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(ca => ca.RequestedSwapRequests)
                .WithOne(sr => sr.RequestedPhysicianCoverageAssignment)
                .HasForeignKey(sr => sr.RequestedPhysicianCoverageAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(ca => ca.TargetedSwapRequests)
               .WithOne(sr => sr.TargetedPhysicianCoverageAssignment)
               .HasForeignKey(sr => sr.TargetedPhysicianCoverageAssignmentId)
               .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(ca => new { ca.PhysicianId, ca.CoverageDate });
        });
       
    }

    private static void ConfigureCoverageGapAlert(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoverageGapAlert>(entity =>
        {
            entity.HasKey(cga => cga.CoverageGapAlertId);

            entity.Property(cga => cga.AlertStatus)
                .HasMaxLength(50);

            entity.Property(cga => cga.AlertReason)
                .HasMaxLength(500);

            entity.Property(cga => cga.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
        modelBuilder.Entity<CoverageGapAlert>()
    .HasIndex(cga => cga.AlertStatus);

    }

    private static void ConfigureSwapRequest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SwapRequest>(entity =>
        {
            entity.HasKey(sr => sr.SwapRequestId);

            entity.Property(sr => sr.RequestStatus)
                .HasMaxLength(50);

            entity.Property(sr => sr.RequestComments)
                .HasMaxLength(500);

            entity.Property(sr => sr.RequestedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }

    private static void ConfigureExternalShiftsData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalShiftsData>(entity =>
        {
            entity.HasKey(esd => esd.ExternalShiftId);
        });
    }

    private static void ConfigureExternalLeavesData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalLeavesData>(entity =>
        {
            entity.HasKey(eld => eld.ExternalLeaveId);

            entity.Property(eld => eld.LeaveReason)
                .HasMaxLength(500);
        });
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.NotificationId);

            entity.Property(n => n.NotificationTitle)
                .HasMaxLength(100);

            entity.Property(n => n.NotificationMessage)
                .HasMaxLength(500);

            entity.Property(n => n.IsRead)
                .HasDefaultValue(false);

            entity.Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }

    private static void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.AuditLogId);

            entity.Property(a => a.ActionType)
                .HasMaxLength(100);

            entity.Property(a => a.EntityName)
                .HasMaxLength(100);

            entity.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                RoleName = "House Supervisor"
            },
            new Role
            {
                RoleId = 2,
                RoleName = "Physician"
            }
        );
    }

    private static void SeedSpecialties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Specialty>().HasData(
            new Specialty
            {
                SpecialtyId = 1,
                SpecialtyName = "Cardiology",
                IsActive = true
            },
            new Specialty
            {
                SpecialtyId = 2,
                SpecialtyName = "Neurology",
                IsActive = true
            },
            new Specialty
            {
                SpecialtyId = 3,
                SpecialtyName = "Orthopedics",
                IsActive = true
            },
            new Specialty
            {
                SpecialtyId = 4,
                SpecialtyName = "Emergency Medicine",
                IsActive = true
            },
            new Specialty
            {
                SpecialtyId = 5,
                SpecialtyName = "Radiology",
                IsActive = true
            },
            new Specialty
            {
                SpecialtyId = 6,
                SpecialtyName = "Anesthesiology",
                IsActive = true
            },
            new Specialty
            {
                SpecialtyId = 7,
                SpecialtyName = "General Surgery",
                IsActive = true
            }
        );
    }
}