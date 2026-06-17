using LoanApplicationSystem.Data;
using LoanApplicationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApplicationSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            var allApplications = _context.LoanApplications.AsQueryable();

            ViewBag.TotalCount = await allApplications.CountAsync();
            ViewBag.NewCount = await allApplications.CountAsync(x => x.Status == "Złożony");
            ViewBag.InProgressCount = await allApplications.CountAsync(x => x.Status == "W trakcie analizy");
            ViewBag.AcceptedCount = await allApplications.CountAsync(x => x.Status == "Zaakceptowany");
            ViewBag.RejectedCount = await allApplications.CountAsync(x => x.Status == "Odrzucony");
            ViewBag.InvalidDataCount = await allApplications.CountAsync(x => x.Status == "Błędne dane");

            var query = _context.LoanApplications
                .Include(x => x.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.FullName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.PhoneNumber.Contains(search) ||
                    x.Purpose.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            var applications = await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(applications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var application = await _context.LoanApplications
                .Include(x => x.User)
                .Include(x => x.DecisionHistory)
                .ThenInclude(x => x.ChangedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus, string? comment)
        {
            var allowedStatuses = new[]
            {
                "Złożony",
                "W trakcie analizy",
                "Zaakceptowany",
                "Odrzucony",
                "Błędne dane"
            };

            if (!allowedStatuses.Contains(newStatus))
            {
                TempData["ErrorMessage"] = "Wybrano nieprawidłowy status wniosku.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var application = await _context.LoanApplications
                .FirstOrDefaultAsync(x => x.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            var isAutomaticallyRejected =
                application.CreditDecision == "Brak zdolności kredytowej" ||
                application.CreditScore < 55;

            if (isAutomaticallyRejected &&
                newStatus != "Odrzucony" &&
                newStatus != "Błędne dane")
            {
                TempData["ErrorMessage"] =
                    "Wniosek automatycznie odrzucony przez system nie może zostać zaakceptowany. Możesz pozostawić status Odrzucony albo oznaczyć go jako Błędne dane.";

                return RedirectToAction(nameof(Details), new { id });
            }

            var oldStatus = application.Status;

            if (oldStatus == newStatus)
            {
                TempData["ErrorMessage"] = "Wybrany status jest taki sam jak aktualny.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var adminId = _userManager.GetUserId(User);

            application.Status = newStatus;
            application.UpdatedAt = DateTime.Now;

            var history = new DecisionHistory
            {
                LoanApplicationId = application.Id,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Comment = comment?.Trim(),
                ChangedByUserId = adminId,
                ChangedAt = DateTime.Now
            };

            _context.DecisionHistories.Add(history);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Status wniosku został zmieniony.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}