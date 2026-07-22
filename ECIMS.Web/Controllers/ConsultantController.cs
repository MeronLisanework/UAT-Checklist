using System.Security.Claims;
using ECIMS.Web.Data;
using ECIMS.Web.Models.Entities;
using ECIMS.Web.Models.Enums;
using ECIMS.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECIMS.Web.Controllers
{
    [Authorize(Roles = RoleNames.FunctionalConsultant)]
    public class ConsultantController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const int PageSize = 6;

        public ConsultantController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index(string? search, string status = "All", int page = 1)
        {
            var userId = CurrentUserId;

            var projects = await _db.Projects
                .Where(p => p.ConsultantId == userId)
                .Include(p => p.Branch).ThenInclude(b => b.Customer)
                .Include(p => p.Attempts).ThenInclude(a => a.Results).ThenInclude(r => r.History)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var items = projects.Select(p =>
            {
                var lastUpdated = p.CreatedAt;

                foreach (var attempt in p.Attempts)
                {
                    if (attempt.StartedDate > lastUpdated) lastUpdated = attempt.StartedDate;
                    foreach (var result in attempt.Results)
                    {
                        if (result.ExecutedDate > lastUpdated) lastUpdated = result.ExecutedDate;
                        foreach (var h in result.History)
                            if (h.EditedAt > lastUpdated) lastUpdated = h.EditedAt;
                    }
                }

                return new ConsultantProjectListItem
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    CustomerName = p.Branch.Customer.CustomerName,
                    BranchName = p.Branch.BranchName,
                    Status = p.Status,
                    StatusLabel = ProjectStatusDisplay.Label(p.Status),
                    StatusCssClass = ProjectStatusDisplay.CssClass(p.Status),
                    LastUpdated = lastUpdated
                };
            }).ToList();

            var model = new ConsultantDashboardViewModel
            {
                Search = search,
                StatusFilter = string.IsNullOrEmpty(status) ? "All" : status,
                Page = page < 1 ? 1 : page,
                PageSize = PageSize,
                TotalCount = items.Count,
                InReviewCount = items.Count(i => ProjectStatusDisplay.FilterBucket(i.Status) == "InReview"),
                DeclinedCount = items.Count(i => ProjectStatusDisplay.FilterBucket(i.Status) == "Declined"),
                CompletedCount = items.Count(i => ProjectStatusDisplay.FilterBucket(i.Status) == "Completed")
            };

            var filtered = items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                filtered = filtered.Where(i =>
                    i.ProjectName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    i.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    i.BranchName.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            if (model.StatusFilter != "All")
            {
                filtered = filtered.Where(i => ProjectStatusDisplay.FilterBucket(i.Status) == model.StatusFilter);
            }

            var filteredList = filtered.OrderByDescending(i => i.LastUpdated).ToList();
            model.TotalFilteredCount = filteredList.Count;

            model.Projects = filteredList
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> ProjectDetails(int id)
{
    var userId = CurrentUserId;

    var project = await _db.Projects
        .Include(p => p.Branch).ThenInclude(b => b.Customer)
        .Include(p => p.ProjectManager)
        .Include(p => p.CreatedBy)
        .Include(p => p.Attempts).ThenInclude(a => a.Results).ThenInclude(r => r.History)
        .Include(p => p.Attempts).ThenInclude(a => a.InitiatedBy)
        .Include(p => p.Attempts).ThenInclude(a => a.DecidedBy)
        .Include(p => p.Attempts).ThenInclude(a => a.Signatures).ThenInclude(s => s.SignedBy)
        .FirstOrDefaultAsync(p => p.ProjectId == id && p.ConsultantId == userId);

    if (project is null) return NotFound();

    var orderedAttempts = project.Attempts.OrderBy(a => a.AttemptNumber).ToList();
    var current = orderedAttempts.LastOrDefault();

    var model = new ProjectDetailsViewModel
    {
        ProjectId = project.ProjectId,
        ProjectName = project.ProjectName,
        CustomerName = project.Branch.Customer.CustomerName,
        BranchName = project.Branch.BranchName,
        ProjectManagerName = project.ProjectManager.FullName,
        CustomerContactName = project.Branch.SiteContactName,
        Status = project.Status,
        StatusLabel = ProjectStatusDisplay.Label(project.Status),
        StatusCssClass = ProjectStatusDisplay.CssClass(project.Status),
        StartDate = project.StartDate,
        EndDate = project.EndDate
    };

    if (current is not null)
    {
        model.CurrentAttemptId = current.AttemptId;
        model.CurrentAttemptNumber = current.AttemptNumber;
        model.ProgressTotal = current.Results.Count;
        model.ProgressCompleted = current.Results.Count(r => r.PassStatus != PassStatus.Pending);
    }

    model.CanStartFirstAttempt = project.Status == ProjectStatus.Pending && current is null;
    model.CanContinueChecklist = current is not null && current.OverallStatus == AttemptOverallStatus.InProgress;
model.CanSendToCustomer = current is not null
    && current.OverallStatus == AttemptOverallStatus.InProgress;
    model.IsAwaitingCustomerReview = project.Status == ProjectStatus.AwaitingCustomerReview;
    model.CanSignAsConsultant = project.Status == ProjectStatus.AwaitingConsultantSignature;
    model.IsAwaitingPmApproval = project.Status == ProjectStatus.AwaitingPmSignature;
    model.IsCompleted = project.Status == ProjectStatus.Completed;

    var previousDeclined = orderedAttempts.Count > 1 ? orderedAttempts[^2] : null;
    if (project.Status == ProjectStatus.Declined
        || (previousDeclined is not null
            && previousDeclined.OverallStatus == AttemptOverallStatus.Declined
            && current is not null
            && current.OverallStatus == AttemptOverallStatus.InProgress))
    {
        var declinedAttempt = project.Status == ProjectStatus.Declined ? current : previousDeclined;

        if (declinedAttempt is not null)
        {
            var historyOnDecline = declinedAttempt.Results.SelectMany(r => r.History).ToList();
            var changedItemIds = historyOnDecline
                .Where(h => h.PreEditPassStatus != h.PostEditPassStatus)
                .Select(h => h.Result.MasterItemId)
                .Distinct()
                .ToList();
            var flaggedOnlyItemIds = historyOnDecline
                .Where(h => h.PreEditPassStatus == h.PostEditPassStatus)
                .Select(h => h.Result.MasterItemId)
                .Distinct()
                .Except(changedItemIds)
                .ToList();

            var touchedItemIds = changedItemIds.Concat(flaggedOnlyItemIds).Distinct().ToList();

            var unresolvedCount = current is null
                ? touchedItemIds.Count
                : current.Results.Count(r => touchedItemIds.Contains(r.MasterItemId) && r.PassStatus == PassStatus.Pending);

            model.Decline = new DeclineInfo
            {
                DeclinedDate = declinedAttempt.DecidedDate ?? declinedAttempt.StartedDate,
                FlaggedCount = flaggedOnlyItemIds.Count,
                ChangedCount = changedItemIds.Count,
                UnresolvedCount = unresolvedCount
            };
        }
    }

    var activity = new List<ActivityItem>
    {
        new ActivityItem { Title = "Project created and assigned", ActorName = project.CreatedBy.FullName, When = project.CreatedAt }
    };

    foreach (var attempt in orderedAttempts)
    {
        activity.Add(new ActivityItem
        {
            Title = attempt.AttemptNumber == 1 ? "Checklist started" : $"New attempt started (#{attempt.AttemptNumber})",
            ActorName = attempt.InitiatedBy.FullName,
            When = attempt.StartedDate
        });

        if (attempt.SubmittedDate is not null)
        {
            activity.Add(new ActivityItem
            {
                Title = $"Checklist completed, attempt #{attempt.AttemptNumber}",
                ActorName = attempt.InitiatedBy.FullName,
                When = attempt.SubmittedDate.Value
            });
        }

        if (attempt.DecidedDate is not null && attempt.DecidedBy is not null)
        {
            var verb = attempt.OverallStatus == AttemptOverallStatus.Declined ? "declined" : "accepted";
            activity.Add(new ActivityItem
            {
                Title = $"Customer {verb} attempt #{attempt.AttemptNumber}",
                ActorName = attempt.DecidedBy.FullName,
                When = attempt.DecidedDate.Value
            });
        }

        foreach (var sig in attempt.Signatures.OrderBy(s => s.DateStamped))
        {
            activity.Add(new ActivityItem
            {
                Title = $"{sig.SignatoryRole} signed",
                ActorName = sig.SignedBy.FullName,
                When = sig.DateStamped
            });
        }
    }

    var sorted = activity.OrderByDescending(a => a.When).ToList();
    if (sorted.Count > 0) sorted[0].IsLatest = true;
    model.Activity = sorted;

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SubmitForReview(int attemptId)
{
    var userId = CurrentUserId;

    var attempt = await _db.ProjectUatAttempts
        .Include(a => a.Project)
        .Include(a => a.Results)
        .FirstOrDefaultAsync(a => a.AttemptId == attemptId && a.Project.ConsultantId == userId);

    if (attempt is null) return NotFound();

    attempt.SubmittedDate = DateTime.UtcNow;
    attempt.OverallStatus = AttemptOverallStatus.SubmittedForReview;
    attempt.Project.Status = ProjectStatus.AwaitingCustomerReview;

    await _db.SaveChangesAsync();

    return RedirectToAction("ProjectDetails", new { id = attempt.ProjectId });
}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartAttempt(int projectId)
        {
            var userId = CurrentUserId;

            var project = await _db.Projects
                .Include(p => p.Attempts)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.ConsultantId == userId);

            if (project is null) return NotFound();

            var nextAttemptNumber = project.Attempts.Any() ? project.Attempts.Max(a => a.AttemptNumber) + 1 : 1;

            var masterItems = await _db.UatMasterItems.ToListAsync();

            var attempt = new ProjectUatAttempt
            {
                ProjectId = project.ProjectId,
                AttemptNumber = nextAttemptNumber,
                StartedDate = DateTime.UtcNow,
                OverallStatus = AttemptOverallStatus.InProgress,
                InitiatedById = userId
            };

            _db.ProjectUatAttempts.Add(attempt);
            await _db.SaveChangesAsync();

            foreach (var item in masterItems)
            {
                _db.ProjectUatResults.Add(new ProjectUatResult
                {
                    AttemptId = attempt.AttemptId,
                    MasterItemId = item.MasterItemId,
                    PassStatus = PassStatus.Pending,
                    ExecutedById = userId,
                    ExecutedDate = DateTime.UtcNow
                });
            }

            project.Status = ProjectStatus.Active;
            await _db.SaveChangesAsync();

            return RedirectToAction("Attempt", new { id = attempt.AttemptId });
        }

        // Stub — the real pass/fail checklist execution UI is the next step
       public async Task<IActionResult> Attempt(int id)
{
    var model = await BuildAttemptExecuteViewModel(id, CurrentUserId);
    if (model is null) return NotFound();
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SaveChecklist(int attemptId, List<ChecklistItemInput> items)
{
    var userId = CurrentUserId;

    var attempt = await _db.ProjectUatAttempts
        .Include(a => a.Project)
        .Include(a => a.Results)
        .FirstOrDefaultAsync(a => a.AttemptId == attemptId && a.Project.ConsultantId == userId);

    if (attempt is null) return NotFound();

    foreach (var input in items)
    {
        if (input.PassStatus == PassStatus.Fail && string.IsNullOrWhiteSpace(input.Comment))
        {
            ModelState.AddModelError("", "A comment is required for every item marked Fail.");
        }
    }

    if (!ModelState.IsValid)
    {
        var reloaded = await BuildAttemptExecuteViewModel(attemptId, userId);
        return View("Attempt", reloaded);
    }

    foreach (var input in items)
    {
        var result = attempt.Results.First(r => r.ResultId == input.ResultId);
        result.PassStatus = input.PassStatus;
        result.Comment = input.Comment;

        if (input.EvidenceFile is not null && input.EvidenceFile.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(input.EvidenceFile.FileName)}";
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "evidence");
            Directory.CreateDirectory(folder);
            var savePath = Path.Combine(folder, fileName);
            using var stream = new FileStream(savePath, FileMode.Create);
            await input.EvidenceFile.CopyToAsync(stream);
            result.EvidencePath = $"/uploads/evidence/{fileName}";
        }

        result.ExecutedById = userId;
        result.ExecutedDate = DateTime.UtcNow;
    }

    await _db.SaveChangesAsync();

    return RedirectToAction("ProjectDetails", new { id = attempt.ProjectId });
}

private async Task<AttemptExecuteViewModel?> BuildAttemptExecuteViewModel(int attemptId, int userId)
{
    var attempt = await _db.ProjectUatAttempts
        .Include(a => a.Project).ThenInclude(p => p.Branch).ThenInclude(b => b.Customer)
        .Include(a => a.Project).ThenInclude(p => p.ProjectManager)
        .Include(a => a.Results).ThenInclude(r => r.MasterItem).ThenInclude(m => m.Section)
        .FirstOrDefaultAsync(a => a.AttemptId == attemptId && a.Project.ConsultantId == userId);

    if (attempt is null) return null;

    return new AttemptExecuteViewModel
    {
        AttemptId = attempt.AttemptId,
        ProjectId = attempt.ProjectId,
        AttemptNumber = attempt.AttemptNumber,
        ProjectName = attempt.Project.ProjectName,
        CustomerName = attempt.Project.Branch.Customer.CustomerName,
        BranchName = attempt.Project.Branch.BranchName,
        ProjectManagerName = attempt.Project.ProjectManager.FullName,
        StartDate = attempt.Project.StartDate,
        TotalItems = attempt.Results.Count,
        CompletedItems = attempt.Results.Count(r => r.PassStatus != PassStatus.Pending),
        Sections = attempt.Results
            .GroupBy(r => new { r.MasterItem.Section.SectionId, r.MasterItem.Section.SectionName })
            .OrderBy(g => g.Key.SectionId)
            .Select(g => new ChecklistSectionGroup
            {
                SectionName = g.Key.SectionName,
                Items = g.Select(r => new ChecklistItemInput
                {
                    ResultId = r.ResultId,
                    TestDescription = r.MasterItem.TestDescription,
                    PassStatus = r.PassStatus,
                    Comment = r.Comment,
                    EvidencePath = r.EvidencePath
                }).ToList()
            }).ToList()
    };
}
    }
}