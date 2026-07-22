using System.Security.Claims;
using ECIMS.Web.Data;
using ECIMS.Web.Models.Enums;
using ECIMS.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECIMS.Web.Controllers
{
    [Authorize(Roles = RoleNames.CustomerRepresentative)]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Dashboard()
        {
            var user = await _db.Users.FirstAsync(u => u.UserId == CurrentUserId);

            if (user.CustomerId is null)
            {
                return View("NoCustomerLinked");
            }

            var project = await _db.Projects
                .Include(p => p.Branch)
                .Include(p => p.ProjectManager)
                .Include(p => p.Consultant)
                .Include(p => p.Attempts).ThenInclude(a => a.Results)
                .Where(p => p.Branch.CustomerId == user.CustomerId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (project is null)
            {
                return View("NoProject");
            }

            var orderedAttempts = project.Attempts.OrderByDescending(a => a.AttemptNumber).ToList();
            var current = orderedAttempts.FirstOrDefault();

            var model = new CustomerDashboardViewModel
            {
                CustomerRepName = user.FullName,
                ProjectId = project.ProjectId,
                BranchName = project.Branch.BranchName,
                ProjectManagerName = project.ProjectManager.FullName,
                ConsultantName = project.Consultant.FullName,
                StartDate = project.StartDate,
                Status = project.Status,
                StatusLabel = ProjectStatusDisplay.Label(project.Status),
                StatusCssClass = ProjectStatusDisplay.CssClass(project.Status),
                CurrentAttemptId = current?.AttemptId,
                ProgressTotal = current?.Results.Count ?? 0,
                ProgressCompleted = current?.Results.Count(r => r.PassStatus != PassStatus.Pending) ?? 0,
                ShowReviewBanner = current is not null && current.OverallStatus == AttemptOverallStatus.SubmittedForReview,
                BannerSentDate = current?.SubmittedDate
            };

            model.History = orderedAttempts.Select(a => new CustomerHistoryRow
            {
                AttemptId = a.AttemptId,
                AttemptNumber = a.AttemptNumber,
                StatusLabel = a.OverallStatus switch
                {
                    AttemptOverallStatus.InProgress => "In Progress",
                    AttemptOverallStatus.SubmittedForReview => "In Review",
                    AttemptOverallStatus.Declined => "Declined",
                    AttemptOverallStatus.Accepted => "Accepted",
                    _ => a.OverallStatus.ToString()
                },
                StatusCssClass = a.OverallStatus switch
                {
                    AttemptOverallStatus.InProgress => "status-active",
                    AttemptOverallStatus.SubmittedForReview => "status-inreview",
                    AttemptOverallStatus.Declined => "status-declined",
                    AttemptOverallStatus.Accepted => "status-completed",
                    _ => "status-pending"
                },
                Date = a.DecidedDate ?? a.SubmittedDate ?? a.StartedDate
            }).ToList();

            return View(model);
        }
    }
}