using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string ProjectManagerName { get; set; } = string.Empty;
        public string CustomerContactName { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public string StatusCssClass { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? CurrentAttemptId { get; set; }
        public int? CurrentAttemptNumber { get; set; }
        public int ProgressCompleted { get; set; }
        public int ProgressTotal { get; set; }

        public bool CanStartFirstAttempt { get; set; }
        public bool CanContinueChecklist { get; set; }
        public bool CanSendToCustomer { get; set; }
        public bool IsAwaitingCustomerReview { get; set; }
        public bool CanSignAsConsultant { get; set; }
        public bool IsAwaitingPmApproval { get; set; }
        public bool IsCompleted { get; set; }

        public DeclineInfo? Decline { get; set; }
        public List<ActivityItem> Activity { get; set; } = new();
    }

    public class DeclineInfo
    {
        public DateTime DeclinedDate { get; set; }
        public int FlaggedCount { get; set; }
        public int ChangedCount { get; set; }
        public int UnresolvedCount { get; set; }
    }

    public class ActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string ActorName { get; set; } = string.Empty;
        public DateTime When { get; set; }
        public bool IsLatest { get; set; }
    }
}