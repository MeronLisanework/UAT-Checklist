using System.ComponentModel.DataAnnotations;

namespace ECIMS.Web.Models.Entities
{
    public class UatMasterItem
    {
        public int MasterItemId { get; set; }

        public int SectionId { get; set; }
        public UatSection Section { get; set; } = null!;

        [Required]
        public string TestDescription { get; set; } = string.Empty;
    }
}