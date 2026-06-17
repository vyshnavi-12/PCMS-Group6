using PCMS_Backend.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PCMS_Backend.Models;

    public class SwapRequest
    {
        
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

        public CoverageAssignment CoverageAssignment { get; set; } = default!;

        public Physician RequestedByPhysician { get; set; } = default!;

        public Physician TargetPhysician { get; set; } = default!;

        public User? ReviewedByUser { get; set; }
  
}