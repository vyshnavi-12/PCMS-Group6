using Microsoft.EntityFrameworkCore;
using PCMS_Backend.Data;
using PCMS_Backend.DTOs;
using PCMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PCMS_Backend.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly PcmsDbContext _context;

        public AuditLogRepository(PcmsDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AuditLogDto>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new AuditLogDto
                {
                    AuditLogId = l.AuditLogId,
                    ActionType = l.ActionType,
                    EntityName = l.EntityName,
                    EntityRecordId = l.EntityRecordId,
                    PerformedByUserId = l.PerformedByUserId,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }


        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
