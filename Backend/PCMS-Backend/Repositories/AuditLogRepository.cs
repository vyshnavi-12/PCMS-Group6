using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly PcmsDbContext _context;

        public AuditLogRepository(PcmsDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(l => l.PerformedByUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
