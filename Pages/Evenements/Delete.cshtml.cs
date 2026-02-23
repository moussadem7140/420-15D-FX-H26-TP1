using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Authorization;

namespace _420_15D_FX_H26_TP1.Pages.Evenements
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;

        public DeleteModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Evenement Evenement { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evenement = await _context.evenements.FirstOrDefaultAsync(m => m.Id == id);

            if (evenement is not null)
            {
                Evenement = evenement;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            Evenement = await _context.evenements.FindAsync(id);
            if (Evenement != null)
            {// Au lieu de supprimer l'événement de la base de données, on le marque comme archivé comme discuté en classe.
                if (Evenement.IsArchived)
                {
                    TempData["ErrorMessage"] = "L'événement est déjà archivé.";
                    return RedirectToPage("/Index");
                }
                Evenement.IsArchived = true;
                _context.Attach(Evenement).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "L'événement a été archivé avec succès.";

            }

            return RedirectToPage("/Index");
        }
    }
}
