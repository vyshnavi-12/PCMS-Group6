namespace PCMS_Backend.DTOs
{
    public class AuditLogDto
    {
        public int AuditLogId { get; set; }
        public string ActionType { get; set; } = default!;
        public string EntityName { get; set; } = default!;
        public int EntityRecordId { get; set; }
        public int PerformedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
