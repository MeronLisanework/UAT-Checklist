using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.ViewModels
{
    public class ConsultantProjectListItem
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public string StatusCssClass { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}