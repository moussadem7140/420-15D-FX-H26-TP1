using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _420_15D_FX_H26_TP1.Pages.Evenements
{
    public class IndexModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;
        //Le supportsGet vaa me permettre d'utiliser une variable obtenu avec get dans ma methode
        //Je voulais faire la mathode en Get pour respecter la différence entre Post et Get 
        [BindProperty(SupportsGet =true)]
        public string motCle {  get; set; }

        public IndexModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Evenement> Evenement { get;set; }

        public SelectList Categories { get; set; }

        [BindProperty(SupportsGet =true)]
        public Guid? CategorieCle { get; set; }

        public async Task OnGetAsync()
        {
            ViewData["titre"] = "Les Evenements";

            if (!string.IsNullOrEmpty(motCle) || CategorieCle != null)
            {
                if (CategorieCle == null)
                {
                    Evenement = await _context.evenements
                  .Include(e => e.Categorie)
                  .Include(e => e.Organisateur)
                  .Include(e => e.Participants)
                  .ThenInclude(p => p.Utilisateur)
                  .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && (e.Nom.Contains(motCle.Trim()) || e.Description.Contains(motCle.Trim())))
                  .ToListAsync();
                    Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                }
                else if (string.IsNullOrEmpty(motCle))
                {
                    Evenement = await _context.evenements
                  .Include(e => e.Categorie)
                  .Include(e => e.Organisateur)
                  .Include(e => e.Participants)
                  .ThenInclude(p => p.Utilisateur)
                  .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && e.CategorieId == CategorieCle)
                  .ToListAsync();
                    Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                }
                else
                {
                   Evenement = await _context.evenements
                 .Include(e => e.Categorie)
                 .Include(e => e.Organisateur)
                 .Include(e => e.Participants)
                 .ThenInclude(p => p.Utilisateur)
                 .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && (e.Nom.Contains(motCle.Trim()) || e.Description.Contains(motCle.Trim())) && e.CategorieId == CategorieCle)
                 .ToListAsync();
                }
     
            }
            else
            {
                Evenement = await _context.evenements
               .Include(e => e.Categorie)
               .Include(e => e.Organisateur)
               .Include(e => e.Participants)
               .ThenInclude(p => p.Utilisateur)
               .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false)
               .ToListAsync();
                Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
            }
           
        }
      
        public async Task OnGetParticipations()
        {
            Evenement = await _context.evenements
               .Include(e => e.Categorie)
               .Include(e => e.Organisateur)
               .Include(e => e.Participants)
               .ThenInclude(p => p.Utilisateur)
               .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && (e.Participants.Any(p=> p.Utilisateur.UserName== User.Identity.Name))&& e.Organisateur.UserName != User.Identity.Name)
               .ToListAsync();
            ViewData["titre"] = "La liste de mes participations";
            Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");

        }
        public async Task OnGetMesEvenements()
        {
            Evenement = await _context.evenements
               .Include(e => e.Categorie)
               .Include(e => e.Organisateur)
               .Include(e => e.Participants)
               .ThenInclude(p => p.Utilisateur)
               .Where(e=>e.Organisateur.UserName==User.Identity.Name)
               .ToListAsync();
            ViewData["titre"] = "Historique de mes évenements";
            Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");

        }

    }
}
