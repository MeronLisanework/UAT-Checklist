namespace ECIMS.Web.Models.Enums
{
    public static class ProjectStatusDisplay
    {
        public static string Label(ProjectStatus status) => status switch
        {
            ProjectStatus.Pending => "Not Started",
            ProjectStatus.Active => "In Progress",
            ProjectStatus.AwaitingCustomerReview => "In Review",
            ProjectStatus.AwaitingConsultantSignature => "Signature Required",
            ProjectStatus.AwaitingPmSignature => "Awaiting PM Approval",
            ProjectStatus.Declined => "Declined",
            ProjectStatus.Completed => "Completed",
            _ => status.ToString()
        };

        public static string CssClass(ProjectStatus status) => status switch
        {
            ProjectStatus.Pending => "status-pending",
            ProjectStatus.Active => "status-active",
            ProjectStatus.AwaitingCustomerReview => "status-inreview",
            ProjectStatus.AwaitingConsultantSignature => "status-signature",
            ProjectStatus.AwaitingPmSignature => "status-signature",
            ProjectStatus.Declined => "status-declined",
            ProjectStatus.Completed => "status-completed",
            _ => "status-pending"
        };

        public static string FilterBucket(ProjectStatus status) => status switch
        {
            ProjectStatus.Pending => "Draft",
            ProjectStatus.Active => "Draft",
            ProjectStatus.AwaitingCustomerReview => "InReview",
            ProjectStatus.AwaitingConsultantSignature => "InReview",
            ProjectStatus.AwaitingPmSignature => "InReview",
            ProjectStatus.Declined => "Declined",
            ProjectStatus.Completed => "Completed",
            _ => "Draft"
        };
    }
}