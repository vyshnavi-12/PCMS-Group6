using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.DTOs;
using PCMS_Backend.Models;

namespace PCMS_Backend.Services.Scheduling.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly PcmsDbContext _context;

        public RecommendationRepository(PcmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Physician>> GetPhysiciansAsync()
        {
            return await _context.Physicians
                .Include(p => p.User)
                .Include(p => p.PhysicianSpecialtyMaps)
                .ToListAsync();
        }

        public async Task<List<ExternalLeavesData>> GetLeavesAsync()
        {
            return await _context.ExternalLeavesData
                .ToListAsync();
        }

        public async Task<List<ExternalShiftsData>> GetExternalShiftsAsync()
        {
            return await _context.ExternalShiftsData
                .ToListAsync();
        }

        public async Task<List<CoverageAssignment>> GetAssignmentsAsync(
            DateOnly startDate,
            DateOnly endDate)
        {
            return await _context.CoverageAssignments
                .Where(a =>
                    a.CoverageDate >= startDate &&
                    a.CoverageDate <= endDate)
                .ToListAsync();
        }

        public async Task<List<PhysicianWorkloadDto>>
            GetPhysicianWorkloadsAsync()
        {
            var fromDate =
                DateTime.UtcNow.AddDays(-60);

            return await _context.Physicians
                .Select(p => new PhysicianWorkloadDto
                {
                    PhysicianId = p.PhysicianId,

                    JoinDate = p.User.CreatedAt,

                    MorningShiftCount =
                        _context.CoverageAssignments
                            .Count(a =>
                                a.PhysicianId ==
                                    p.PhysicianId &&
                                a.CoverageDate >=
                                    DateOnly.FromDateTime(fromDate) &&
                                a.ShiftType == "Day"),

                    NightShiftCount =
                        _context.CoverageAssignments
                            .Count(a =>
                                a.PhysicianId ==
                                    p.PhysicianId &&
                                a.CoverageDate >=
                                    DateOnly.FromDateTime(fromDate) &&
                                a.ShiftType == "Night")
                })
                .ToListAsync();
        }
    }
}