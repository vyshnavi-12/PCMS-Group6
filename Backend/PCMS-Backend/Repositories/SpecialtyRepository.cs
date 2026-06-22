using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories;

public class SpecialtyRepository : ISpecialtyRepository
{
    private readonly PcmsDbContext _context;

    public SpecialtyRepository(PcmsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Specialty>> GetAllAsync()
    {
        return await _context.Specialties
            .AsNoTracking()
            .OrderBy(s => s.SpecialtyName)
            .ToListAsync();
    }
}