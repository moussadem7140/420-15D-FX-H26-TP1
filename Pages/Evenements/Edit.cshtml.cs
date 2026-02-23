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
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public EditModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }

        [BindProperty]
        public Evenement Evenement { get; set; }

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }
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
           Categories = new SelectList(_context.Categories.Where(e=> !e.IsArchived), "Id", "Nom");
            return Page();
           
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if(_context.evenements.Any(e => e.Nom ==Evenement.Nom &&
                                            e.Adresse== Evenement.Adresse &&
                                            e.DateDebut== Evenement.DateDebut &&
                                            e.DateFin == Evenement.DateFin &&
                                            e.codePostal== Evenement.codePostal &&
                                            e.CategorieId== Evenement.CategorieId &&
                                            e.Ville== Evenement.Ville &&
                                            e.Description == Evenement.Description) && ImageUpload==null)
            {
                TempData["SuccessMessage"] = "L'événement n'a finalement pas été modifié";
                return RedirectToPage("/Index");
            }
            if (!ModelState.IsValid)
            {
                Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                return Page();
            }
            if (Evenement.DateDebut < DateTime.Now)
            {
                ModelState.AddModelError("Evenement.DateDebut", "La date de l'événement doit être dans le futur.");
                Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                return Page();
            }
            if (Evenement.DateFin <= Evenement.DateDebut)
            {
                ModelState.AddModelError("Evenement.DateFin", "La date de fin doit être après la date de début.");
                Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                return Page();
            }
            if (_context.evenements.Any(e => e.Adresse == Evenement.Adresse && (Evenement.DateFin > e.DateDebut && Evenement.DateDebut < e.DateFin) && e.IsArchived == false && Evenement.Id != e.Id))
            {

                ModelState.AddModelError("", "Il y a déjà un événement prévu à cette adresse pendant cette période.");
                Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                return Page();
            }
            if (!(ImageUpload == null))
            {
                // Validation du type de fichier
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(ImageUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ImageUpload",
                        "Seuls les fichiers JPG, PNG ou WEBP sont autorisés."
                    );
                    Categories = new SelectList(_context.Categories, "Id", "Nom");
                    return Page();
                }

                // Récupération du dossier de téléversement
                var uploadFolder = _configuration["Images:UploadPath"]; //Obtient le chemin d'accès dans le appsettings.json 
                var uploadPath = Path.Combine(_environment.WebRootPath, uploadFolder);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }


                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);


                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // Génération d’un nom unique
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageUpload.CopyToAsync(stream);

                Evenement.Image = fileName;
            }

        
            _context.Attach(Evenement).State = EntityState.Modified;
             await _context.SaveChangesAsync();
            
           
            TempData["SuccessMessage"] = "L'événement a été mis à jour avec succès.";

            return RedirectToPage("/Index");
        }
    }
}
