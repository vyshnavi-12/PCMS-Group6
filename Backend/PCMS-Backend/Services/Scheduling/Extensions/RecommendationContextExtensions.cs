using PCMS_Backend.Services.Scheduling.Models;

namespace PCMS_Backend.Services.Scheduling.Extensions
{
    public static class RecommendationContextExtensions
    {
        public static void RegisterAssignment(
            this RecommendationContext context,
            int physicianId,
            DateOnly coverageDate,
            string shiftType,
            int specialtyId)
        {
            if (!context.AssignmentTracker.ContainsKey(physicianId))
            {
                context.AssignmentTracker[physicianId] =
                    new List<AssignmentInfo>();
            }
            context.AssignmentTracker[physicianId]
            .Add(new AssignmentInfo
            {
                CoverageDate = coverageDate,
                ShiftType = shiftType,
                SpecialtyId = specialtyId
            });

            if (!context.Workloads.ContainsKey(physicianId))
            {
                context.Workloads[physicianId] =
                    new PhysicianWorkload();
            }
            if (string.Equals(
                shiftType,
                "Day",
                StringComparison.OrdinalIgnoreCase))
            {
                context.Workloads[physicianId]
                    .CurrentMorningAssignments++;
            }
            else
            {
                context.Workloads[physicianId]
                    .CurrentNightAssignments++;
            }
        }
    }
}
