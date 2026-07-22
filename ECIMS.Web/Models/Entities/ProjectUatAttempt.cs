using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.Entities
{
    public class ProjectUatAttempt
    {
        public int AttemptId { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int AttemptNumber { get; set; }

        public DateTime StartedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? DecidedDate { get; set; }

        public int? DecidedById { get; set; }
        public User? DecidedBy { get; set; }

        public AttemptOverallStatus OverallStatus { get; set; } = AttemptOverallStatus.InProgress;

        public int InitiatedById { get; set; }
        public User InitiatedBy { get; set; } = null!;

        public ICollection<ProjectUatResult> Results { get; set; } = new List<ProjectUatResult>();
        public ICollection<DigitalSignature> Signatures { get; set; } = new List<DigitalSignature>();
        public AcceptanceCert? Certificate { get; set; }
    }
}