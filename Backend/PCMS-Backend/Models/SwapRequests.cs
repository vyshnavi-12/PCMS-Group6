using System;
using System.ComponentModel.DataAnnotations;

namespace Physician_On_Call_Schedule_Management_System.Models
{
    public class SwapRequests
    {
        [Key]
        public int SwapRequestId { get; set; }

        public int CoverageAssignmentId { get; set; }

        public int RequestedByPhysicianId { get; set; }

        public int TargetPhysicianId { get; set; }

        public string RequestStatus { get; set; } = default!;

        public string RequestComments { get; set; } = default!;

        public DateTime RequestedAt { get; set; }

        public DateTime? RespondedAt { get; set; }

        public int? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}
