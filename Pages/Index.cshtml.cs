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
    public class IndexModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;

        public IndexModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Evenement> Evenement { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Evenement = await _context.evenements
                .Include(e => e.Categorie)
                .Include(e => e.Organisateur)
                .Include(e => e.Participants)
                .ToListAsync();
        }
    }
}
