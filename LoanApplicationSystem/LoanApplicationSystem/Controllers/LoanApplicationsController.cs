using LoanApplicationSystem.Data;
using LoanApplicationSystem.Models;
using LoanApplicationSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApplicationSystem.Controllers
{
    [Authorize]
    public class LoanApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CreditScoringService _creditScoringService;

        public LoanApplicationsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            CreditScoringService creditScoringService)
        {
            _context = context;
            _userManager = userManager;
            _creditScoringService = creditScoringService;
        }

        public async Task<IActionResult> Index()
        {
            var employeeId = _userManager.GetUserId(User);

            var applications = await _context.LoanApplications
                .Where(x => x.UserId == employeeId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(applications);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new LoanApplication());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanApplication application)
        {
            var employeeId = _userManager.GetUserId(User);

            if (employeeId == null)
            {
                return Unauthorized();
            }

            application.Id = 0;
            application.UserId = employeeId;
            application.User = null;
            application.CreatedAt = DateTime.Now;
            application.UpdatedAt = null;
            application.Status = "Złożony";
            application.CreditScore = 0;
            application.CreditDecision = string.Empty;
            application.DisposableIncome = 0;
            application.EstimatedMonthlyInstallment = 0;
            application.DtiPercent = 0;
            application.DecisionHistory = new List<DecisionHistory>();

            application.FullName = application.FullName?.Trim() ?? string.Empty;
            application.Email = application.Email?.Trim() ?? string.Empty;
            application.PhoneNumber = NormalizePhoneNumber(application.PhoneNumber);
            application.Purpose = application.Purpose?.Trim() ?? string.Empty;

            RemoveSystemFieldsFromModelState();

            TryValidateModel(application);

            if (!ModelState.IsValid)
            {
                return View(application);
            }

            _creditScoringService.Calculate(application);

            if (application.CreditDecision == "Brak zdolności kredytowej" || application.CreditScore < 55)
            {
                application.Status = "Odrzucony";

                TempData["SuccessMessage"] =
                    "Wniosek został zapisany i automatycznie odrzucony przez system z powodu niespełnienia minimalnych kryteriów zdolności kredytowej.";
            }
            else
            {
                application.Status = "Złożony";

                TempData["SuccessMessage"] =
                    "Wniosek został zapisany i przekazany do analizy przez przełożonego.";
            }

            _context.LoanApplications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = application.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var employeeId = _userManager.GetUserId(User);

            var application = await _context.LoanApplications
                .Include(x => x.DecisionHistory)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == employeeId);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        private void RemoveSystemFieldsFromModelState()
        {
            ModelState.Remove(nameof(LoanApplication.Id));
            ModelState.Remove(nameof(LoanApplication.UserId));
            ModelState.Remove(nameof(LoanApplication.User));
            ModelState.Remove(nameof(LoanApplication.CreatedAt));
            ModelState.Remove(nameof(LoanApplication.UpdatedAt));
            ModelState.Remove(nameof(LoanApplication.Status));
            ModelState.Remove(nameof(LoanApplication.CreditScore));
            ModelState.Remove(nameof(LoanApplication.CreditDecision));
            ModelState.Remove(nameof(LoanApplication.DisposableIncome));
            ModelState.Remove(nameof(LoanApplication.EstimatedMonthlyInstallment));
            ModelState.Remove(nameof(LoanApplication.DtiPercent));
            ModelState.Remove(nameof(LoanApplication.DecisionHistory));
        }

        private string NormalizePhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }

            var cleaned = phoneNumber.Trim();

            var firstSpaceIndex = cleaned.IndexOf(' ');

            if (firstSpaceIndex <= 0)
            {
                return cleaned;
            }

            var prefix = cleaned.Substring(0, firstSpaceIndex).Trim();
            var numberPart = cleaned.Substring(firstSpaceIndex + 1);

            var digits = new string(numberPart.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(digits))
            {
                return cleaned;
            }

            var groups = new List<string>();

            for (int i = 0; i < digits.Length; i += 3)
            {
                var length = Math.Min(3, digits.Length - i);
                groups.Add(digits.Substring(i, length));
            }

            return $"{prefix} {string.Join(" ", groups)}";
        }
    }
}