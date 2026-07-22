using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.Entities
{
    public class ProjectUatResultHistory
    {
        public int HistoryId { get; set; }

        public int ResultId { get; set; }
        public ProjectUatResult Result { get; set; } = null!;

        public PassStatus PreEditPassStatus { get; set; }
        public PassStatus PostEditPassStatus { get; set; }

        public string EditComment { get; set; } = string.Empty;

        public int EditedById { get; set; }
        public User EditedBy { get; set; } = null!;

        public DateTime EditedAt { get; set; }
    }
}