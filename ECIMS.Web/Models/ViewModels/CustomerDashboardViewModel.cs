using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string CustomerRepName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ProjectManagerName { get; set; } = string.Empty;
        public string ConsultantName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public ProjectStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public string StatusCssClass { get; set; } = string.Empty;

        public int? CurrentAttemptId { get; set; }
        public int ProgressCompleted { get; set; }
        public int ProgressTotal { get; set; }

        public bool ShowReviewBanner { get; set; }
        public DateTime? BannerSentDate { get; set; }

        public List<CustomerHistoryRow> History { get; set; } = new();
    }

    public class CustomerHistoryRow
    {
        public int AttemptId { get; set; }
        public int AttemptNumber { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public string StatusCssClass { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}