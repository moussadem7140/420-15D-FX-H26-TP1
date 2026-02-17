using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _420_15D_FX_H26_TP1.Pages.Evenements
{
    public class CreateModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public IFormFile ImageUpload;

        public CreateModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }
        public SelectList Categories { get; set; }

        public IActionResult OnGet()
        {
        Categories = new SelectList(_context.Categories, "Id", "Nom");
            return Page();
        }

        [BindProperty]
        public Evenement Evenement { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(Evenement.DateDebut < DateTime.Now)
            {
                ModelState.AddModelError("Evenement.DateDebut", "La date de l'événement doit être dans le futur.");
                return Page();
            }
            if(Evenement.DateFin < Evenement.DateDebut)
            {
                ModelState.AddModelError("Evenement.DateFin", "La date de fin doit être après la date de début.");
                return Page();
            }
            if(_context.evenements.Any(e => e.Adresse == Evenement.Adresse && (Evenement.DateFin>=e.DateDebut && Evenement.DateDebut<= e.DateFin) && e.IsArchived==false)){

                ModelState.AddModelError("", "Il y a déjà un événement prévu à cette adresse pendant cette période.");
                return Page();
            }
            if (ImageUpload == null)
            {
                ModelState.AddModelError("ImageUpload", "L'image de l'événement est requise.");
                return Page();
            }

                // Validation du type de fichier
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(ImageUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ImageUpload",
                        "Seuls les fichiers JPG, PNG ou WEBP sont autorisés."
                    );
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
                Evenement.Id = Guid.NewGuid();
                Evenement.OrganisateurId = User.Identity.Name;
                Evenement.IsArchived = false;

            _context.evenements.Add(Evenement);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Événement créé avec succès.";

            return RedirectToPage("/Index");
        }

        
            
        }
    }

