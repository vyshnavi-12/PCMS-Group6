using System.Collections.Generic;
using System.Threading.Tasks;
using PCMS_Backend.DTOs;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services
{
    public interface IAuditLogService
    {
        Task<Result<IReadOnlyList<AuditLogDto>>> GetAuditLogsAsync();
        Task<Result> LogActionAsync(string actionType, string entityName, int entityRecordId, int performedByUserId);
    }
}
