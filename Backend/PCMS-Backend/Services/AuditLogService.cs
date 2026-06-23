using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCMS_Backend.DTOs;
using PCMS_Backend.Models;
using PCMS_Backend.Repositories;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogService(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IReadOnlyList<AuditLogDto>>> GetAuditLogsAsync()
        {
            var logs = await _repository.GetAllAsync();

            var dtos = logs.Select(l => new AuditLogDto
            {
                AuditLogId = l.AuditLogId,
                ActionType = l.ActionType,
                EntityName = l.EntityName,
                EntityRecordId = l.EntityRecordId,
                PerformedByUserId = l.PerformedByUserId,
                // ✅ Keep CreatedAt in UTC
                CreatedAt = l.CreatedAt
            }).ToList();

            return Result<IReadOnlyList<AuditLogDto>>.Ok(dtos, "Audit logs retrieved successfully");
        }

        public async Task<Result> LogActionAsync(string actionType, string entityName, int entityRecordId, int performedByUserId)
        {
            var log = new AuditLog
            {
                ActionType = actionType,
                EntityName = entityName,
                EntityRecordId = entityRecordId,
                PerformedByUserId = performedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(log);

            return Result.Ok("Audit log entry created successfully");
        }
    }
}
