using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;

namespace _420_15D_FX_H26_TP1.Areas.Admin.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;

        public CreateModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Categorie Categorie { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(_context.Categories.Any(c => c.Nom == Categorie.Nom && c.IsArchived == false))
            {
                ModelState.AddModelError("Categorie.Nom", "Une catégorie avec ce nom existe déjà.");
                return Page();
            }

            _context.Categories.Add(Categorie);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Catégorie créée avec succès.";

            return RedirectToPage("./Index");
        }
    }
}
