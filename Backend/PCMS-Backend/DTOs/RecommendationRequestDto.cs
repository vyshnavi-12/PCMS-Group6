
namespace PCMS_Backend.DTOs
{
    public class RecommendationRequestDto
    {
        public int CoverageAssignmentId { get; set; }

        public List<AssignmentUpdateDto> PendingAssignments { get; set; } = new();
    }
}