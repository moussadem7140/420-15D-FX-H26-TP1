using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;

namespace _420_15D_FX_H26_TP1.Pages.Evenements
{
    public class EditModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;

        public EditModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Evenement Evenement { get; set; } = default!;

        [BindProperty]
        public IFormFile ImageUpload { get; set; }
        public SelectList Categories { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evenement =  await _context.evenements.FirstOrDefaultAsync(m => m.Id == id);
            if (evenement == null)
            {
                return NotFound();
            }
            Evenement = evenement;
           Categories = new SelectList(_context.Categories, "Id", "Nom");
            return Page();
           
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Evenement.DateDebut < DateTime.Now)
            {
                ModelState.AddModelError("Evenement.DateDebut", "La date de l'événement doit être dans le futur.");
                return Page();
            }
            if (Evenement.DateFin < Evenement.DateDebut)
            {
                ModelState.AddModelError("Evenement.DateFin", "La date de fin doit être après la date de début.");
                return Page();
            }
            if (_context.evenements.Any(e => e.Adresse == Evenement.Adresse && (Evenement.DateFin >= e.DateDebut && Evenement.DateDebut <= e.DateFin) && e.IsArchived == false))
            {

                ModelState.AddModelError("", "Il y a déjà un événement prévu à cette adresse pendant cette période.");
                return Page();
            }
            _context.Attach(Evenement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvenementExists(Evenement.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EvenementExists(Guid id)
        {
            return _context.evenements.Any(e => e.Id == id);
        }
    }
}
