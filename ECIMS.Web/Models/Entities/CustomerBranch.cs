using System.ComponentModel.DataAnnotations;

namespace ECIMS.Web.Models.Entities
{
    public class CustomerBranch
    {
        public int BranchId { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Required, MaxLength(150)]
        public string BranchName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string SiteContactName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string SiteContactPhone { get; set; } = string.Empty;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
