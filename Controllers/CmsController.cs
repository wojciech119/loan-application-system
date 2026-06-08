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
        public async Task<IActionResult> Edit(int id, string title, string content)
        {
            var page = await _context.CmsPages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Tytuł i treść są wymagane.");
                return View(page);
            }

            page.Title = title;
            page.Content = content;
            page.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Treść została zaktualizowana.";

            return RedirectToAction(nameof(Index));
        }
    }
}