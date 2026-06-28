using PCMS_Backend.DTOs;
using PCMS_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PCMS_Backend.Repositories
{
    public interface IAuditLogRepository
    {
        Task<IReadOnlyList<AuditLogDto>> GetAllAsync();
        Task AddAsync(AuditLog log);
    }
}
