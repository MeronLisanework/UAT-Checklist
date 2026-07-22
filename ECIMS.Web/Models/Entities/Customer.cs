using System.ComponentModel.DataAnnotations;

namespace ECIMS.Web.Models.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required, MaxLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ContactEmail { get; set; } = string.Empty;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public ICollection<CustomerBranch> Branches { get; set; } = new List<CustomerBranch>();
    }
}
