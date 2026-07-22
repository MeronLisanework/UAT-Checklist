using ECIMS.Web.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace ECIMS.Web.Models.ViewModels
{
    public class AttemptExecuteViewModel
    {
        public int AttemptId { get; set; }
        public int ProjectId { get; set; }
        public int AttemptNumber { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string ProjectManagerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int TotalItems { get; set; }
        public int CompletedItems { get; set; }
        public List<ChecklistSectionGroup> Sections { get; set; } = new();
    }

    public class ChecklistSectionGroup
    {
        public string SectionName { get; set; } = string.Empty;
        public List<ChecklistItemInput> Items { get; set; } = new();
    }

    public class ChecklistItemInput
    {
        public int ResultId { get; set; }
        public string TestDescription { get; set; } = string.Empty;
        public PassStatus PassStatus { get; set; }
        public string? Comment { get; set; }
        public string? EvidencePath { get; set; }
        public IFormFile? EvidenceFile { get; set; }
    }
}