using System.ComponentModel.DataAnnotations;

namespace ECIMS.Web.Models.Entities
{
    public class User
    {
        public int? CustomerId { get; set; }
public Customer? Customer { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}