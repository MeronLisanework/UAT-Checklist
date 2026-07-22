using System.ComponentModel.DataAnnotations;
using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.Entities
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required, MaxLength(150)]
        public string ProjectName { get; set; } = string.Empty;

        public int BranchId { get; set; }
        public CustomerBranch Branch { get; set; } = null!;

        public int ProjectManagerId { get; set; }
        public User ProjectManager { get; set; } = null!;

        public int ConsultantId { get; set; }
        public User Consultant { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public ICollection<ProjectUatAttempt> Attempts { get; set; } = new List<ProjectUatAttempt>();
    }
}
