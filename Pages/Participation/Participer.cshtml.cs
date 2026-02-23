using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace _420_15D_FX_H26_TP1.Pages.Participation
{
    [Authorize]
    public class ParticiperModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ParticiperModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Guid EvenementId { get; set; }
        public async Task<IActionResult> OnPost(Guid Id)
        {
            _context.Participations.Add(new _420_15D_FX_H26_TP1.Models.Participation
            {
                Id = Guid.NewGuid(),
                EvenementId = Id,
                UtilisateurId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id
            });
             _context.SaveChanges();
            TempData["SuccessMessage"] = "Vous avez participé à l'événement avec succès !";
            return RedirectToPage("/index");
        }
        public async Task<IActionResult> OnPostAnnuler(Guid Id)
        {
            var P = await _context.Participations.FirstOrDefaultAsync(p => p.EvenementId == Id && p.Utilisateur.UserName == User.Identity.Name);

                _context.Participations.Remove(P);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Votre participation à l'événement a été annulée avec succès !";
                return RedirectToPage("/index");
            
        }
    }
}
