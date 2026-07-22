using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.Entities
{
    public class ProjectUatResult
    {
        public int ResultId { get; set; }

        public int AttemptId { get; set; }
        public ProjectUatAttempt Attempt { get; set; } = null!;

        public int MasterItemId { get; set; }
        public UatMasterItem MasterItem { get; set; } = null!;

        public PassStatus PassStatus { get; set; }

        public string? Comment { get; set; }

        public string? EvidencePath { get; set; }

        public int ExecutedById { get; set; }
        public User ExecutedBy { get; set; } = null!;

        public DateTime ExecutedDate { get; set; }

        public int? LastModifiedById { get; set; }
        public User? LastModifiedBy { get; set; }

        public SignatoryRole? LastModifiedByRole { get; set; }

        public ICollection<ProjectUatResultHistory> History { get; set; } = new List<ProjectUatResultHistory>();
    }
}