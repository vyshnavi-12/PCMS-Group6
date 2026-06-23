using System.Collections.Generic;
using System.Threading.Tasks;
using PCMS_Backend.Models;

namespace PCMS_Backend.Repositories
{
    public interface IAuditLogRepository
    {
        Task<IReadOnlyList<AuditLog>> GetAllAsync();
        Task AddAsync(AuditLog log);
    }
}
