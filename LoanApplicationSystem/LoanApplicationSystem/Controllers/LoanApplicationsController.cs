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

            ModelState.Remove(nameof(LoanApplication.UserId));
            ModelState.Remove(nameof(LoanApplication.User));
            ModelState.Remove(nameof(LoanApplication.Status));
            ModelState.Remove(nameof(LoanApplication.CreditScore));
            ModelState.Remove(nameof(LoanApplication.CreditDecision));
            ModelState.Remove(nameof(LoanApplication.DisposableIncome));
            ModelState.Remove(nameof(LoanApplication.EstimatedMonthlyInstallment));
            ModelState.Remove(nameof(LoanApplication.DtiPercent));
            ModelState.Remove(nameof(LoanApplication.DecisionHistory));

            application.UserId = employeeId;
            application.CreatedAt = DateTime.Now;
            application.Status = "Złożony";

            if (ModelState.IsValid)
            {
                _creditScoringService.Calculate(application);

                _context.LoanApplications.Add(application);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Wniosek klienta został zapisany. System obliczył wstępną zdolność kredytową.";

                return RedirectToAction(nameof(Details), new { id = application.Id });
            }

            return View(application);
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
    }
}