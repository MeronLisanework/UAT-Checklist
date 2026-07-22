using System.ComponentModel.DataAnnotations;

namespace ECIMS.Web.Models.Entities
{
    public class UatSection
    {
        public int SectionId { get; set; }

        [Required, MaxLength(100)]
        public string SectionName { get; set; } = string.Empty;

        public ICollection<UatMasterItem> MasterItems { get; set; } = new List<UatMasterItem>();
    }
}