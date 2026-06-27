namespace PCMS_Backend.DTOs
{
    public class SupervisorDashboardDetailsDto
    {
        public int SwapRequestCount { get; set; }
        public int UnavailableRequestsCount { get; set; }

        public DateOnly NextScheduleDate { get; set; }
    }
}
