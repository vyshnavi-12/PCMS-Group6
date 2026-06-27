using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class SwapRequestRepository : ISwapRequestRepository
{
    private readonly PcmsDbContext _context;

    public SwapRequestRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<CoverageAssignment?> GetAssignmentByIdAsync(
        int coverageAssignmentId
    )
    {
        return await _context.CoverageAssignments
            .Include(a => a.Specialty)
            .Include(a => a.Physician)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a =>
                a.CoverageAssignmentId == coverageAssignmentId);
    }

    public async Task<List<CoverageAssignment>> GetAvailableTargetsAsync(
    int specialtyId,
    int excludedPhysicianId,
    string shiftType,
    int coverageScheduleId
)
    {
        return await _context.CoverageAssignments
            .Include(a => a.Specialty)
            .Include(a => a.Physician)
                .ThenInclude(p => p.User)
            .Where(a =>
                a.SpecialtyId == specialtyId &&
                a.PhysicianId != excludedPhysicianId &&
                a.ShiftType == shiftType &&
                a.CoverageScheduleId == coverageScheduleId)
            .ToListAsync();
    }

    public async Task<bool> HasAssignmentOnDateAsync(
    int physicianId,
    DateOnly date
)
    {
        return await _context.CoverageAssignments
            .AnyAsync(a =>
                a.PhysicianId == physicianId &&
                a.CoverageDate == date);
    }

    public async Task<bool> HasOtherAssignmentOnDateAsync(
    int physicianId,
    DateOnly date,
    int excludedAssignmentId
)
    {
        return await _context.CoverageAssignments
            .AnyAsync(a =>
                a.PhysicianId == physicianId &&
                a.CoverageDate == date &&
                a.CoverageAssignmentId != excludedAssignmentId);
    }

    public async Task CreateAsync(SwapRequest request)
    {
        await _context.SwapRequests.AddAsync(request);
    }

    public async Task<List<SwapRequest>> GetMyRequestsAsync(
    int physicianId
)
    {
        return await _context.SwapRequests
            .Include(s => s.RequestedPhysicianCoverageAssignment)
            .Include(s=>s.TargetedPhysicianCoverageAssignment)

            .Include(s => s.TargetPhysician)
                .ThenInclude(p => p.User)
            .Where(s => s.RequestedByPhysicianId == physicianId)
            .OrderByDescending(s => s.RequestedAt)
            .ToListAsync();
    }

    public async Task<List<SwapRequest>> GetRequestsToMeAsync(
    int physicianId
)
    {
        return await _context.SwapRequests
            .Include(s => s.TargetedPhysicianCoverageAssignment)
            .Include(s=>s.RequestedPhysicianCoverageAssignment)
            .Include(s => s.RequestedByPhysician)
                .ThenInclude(p => p.User)
            .Where(s => s.TargetPhysicianId == physicianId)
            .OrderByDescending(s => s.RequestedAt)
            .ToListAsync();
    }

    public async Task<SwapRequest?> GetByIdAsync(
    int swapRequestId
)
    {
        return await _context.SwapRequests
            .Include(s => s.RequestedByPhysician)
                .ThenInclude(p => p.User)
            .Include(s => s.TargetPhysician)
                .ThenInclude(p => p.User)
            .Include(s => s.RequestedPhysicianCoverageAssignment)
            .Include(s=>s.TargetedPhysicianCoverageAssignment)
            .FirstOrDefaultAsync(s =>
                s.SwapRequestId == swapRequestId);
            }

    public async Task<List<SwapRequest>> GetSupervisorRequestsAsync()
    {
        return await _context.SwapRequests
            .Include(s => s.RequestedByPhysician)
                .ThenInclude(p => p.User)
            .Include(s => s.TargetPhysician)
                .ThenInclude(p => p.User)
            .Include(s => s.RequestedPhysicianCoverageAssignment)
            .Include(s=>s.TargetedPhysicianCoverageAssignment)
            .Where(s => s.RequestStatus == "TARGET_ACCEPTED" || s.ReviewedByUserId != null)
            .OrderByDescending(s => s.RequestedAt)
            .ToListAsync();
    }
    public async Task<int> GetPendingApprovalSwapRequestCount()
    {
        return await _context.SwapRequests
            .Include(s => s.RequestedByPhysician)
                .ThenInclude(p => p.User)
            .Include(s => s.TargetPhysician)
                .ThenInclude(p => p.User)
            .Include(s => s.RequestedPhysicianCoverageAssignment)
            .Include(s => s.TargetedPhysicianCoverageAssignment)
            .Where(s => s.RequestStatus == "TARGET_ACCEPTED" || s.ReviewedByUserId != null)
            .OrderByDescending(s => s.RequestedAt)
            .CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetPhysiciansSwapRequestsCount(int physicianId)
    {
        return await _context.SwapRequests
            .Where(s=>(s.RequestedByPhysicianId == physicianId || s.TargetPhysicianId==physicianId) )
            .CountAsync();
    }

    public async Task<int> GetTargetAcceptedCountAsync()
    {
        return await _context.SwapRequests
            .CountAsync(s => s.RequestStatus == "TARGET_ACCEPTED");
    }
}