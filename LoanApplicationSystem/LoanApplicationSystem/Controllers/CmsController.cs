using LoanApplicationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApplicationSystem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CmsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pages = await _context.CmsPages
                .Where(x => x.Key == "home")
                .OrderBy(x => x.Id)
                .ToListAsync();

            return View(pages);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var page = await _context.CmsPages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string content)
        {
            var page = await _context.CmsPages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Treść ogłoszenia jest wymagana.");
                return View(page);
            }

            page.Content = content.Trim();
            page.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Treść ogłoszenia została zaktualizowana.";

            return RedirectToAction(nameof(Index));
        }
    }
}