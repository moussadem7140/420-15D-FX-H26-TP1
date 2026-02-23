using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;

namespace _420_15D_FX_H26_TP1.Pages.Evenements
{
    public class DetailsModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;

        public DetailsModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context)
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

            var evenement = await _context.evenements.Include(Evenement => Evenement.Categorie)
                                                     .Include(Evenement => Evenement.Organisateur)
                                                     .Include(Evenement => Evenement.Participants)
                                                     .ThenInclude(Participation => Participation.Utilisateur)
                                                     .FirstOrDefaultAsync(m => m.Id == id);
                                                     

            if (evenement is not null)
            {
                Evenement = evenement;

                return Page();
            }

            return NotFound();
        }
    }
}
