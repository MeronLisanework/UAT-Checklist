namespace ECIMS.Web.Models.ViewModels
{
    public class ConsultantDashboardViewModel
    {
        public List<ConsultantProjectListItem> Projects { get; set; } = new();

        public int TotalCount { get; set; }
        public int InReviewCount { get; set; }
        public int DeclinedCount { get; set; }
        public int CompletedCount { get; set; }

        public string? Search { get; set; }
        public string StatusFilter { get; set; } = "All";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalFilteredCount { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalFilteredCount / (double)PageSize);
        public int ShowingFrom => TotalFilteredCount == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int ShowingTo => Math.Min(Page * PageSize, TotalFilteredCount);
    }
}