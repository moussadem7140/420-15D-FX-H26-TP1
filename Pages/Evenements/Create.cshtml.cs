using _420_15D_FX_H26_TP1.Data;
using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]

    public class CreateModel : PageModel
    {
        private readonly _420_15D_FX_H26_TP1.Data.ApplicationDbContext _context;
        public readonly UserManager<Utilisateur> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public CreateModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment environment, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
            _userManager = userManager;
        }
        public SelectList Categories { get; set; }

        [BindProperty]
        public Evenement Evenement { get; set; }

        [BindProperty]

        public IFormFile ImageUpload {  get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");
            return Page();
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Evenement.OrganisateurId = (await _userManager.FindByNameAsync(User.Identity?.Name)).Id;
            Evenement.Image = "parDefaut.jpg";
            if (!ModelState.IsValid)
            {
                Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");

                return Page();

            }
            if(Evenement.DateDebut < DateTime.Now)
            {
                ModelState.AddModelError("Evenement.DateDebut", "La date de l'événement doit être dans le futur.");
                Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");

                return Page();
            }
            if(Evenement.DateFin < Evenement.DateDebut)
            {
                ModelState.AddModelError("Evenement.DateFin", "La date de fin doit être après la date de début.");
                Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");

                return Page();
            }
            //Cette validation vérifie s'il y a déjà un événement prévu à la même adresse pendant la même période, mais je laisse passer le cas ou l'
            //evénement A se termine le même jour que l'événement B commence, ou l'événement A commence le même jour que l'événement B se termine,
            if (_context.evenements.Any(e => e.Adresse == Evenement.Adresse && (Evenement.DateFin > e.DateDebut && Evenement.DateDebut < e.DateFin) && e.IsArchived==false)){

                ModelState.AddModelError("", "Il y a déjà un événement prévu à cette adresse pendant cette période.");
                Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");

                return Page();
            }
            if (ImageUpload == null)
            {
                ModelState.AddModelError("ImageUpload", "L'image de l'événement est requise.");
                Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");

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
                Categories = new SelectList(_context.Categories.Where(e => e.IsArchived == false), "Id", "Nom");

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
                _context.evenements.Add(Evenement);
            await _context.SaveChangesAsync();
            // Ajout de l'organisateur en tant que participant par principe, car il participe à son propre événement
            _context.Participations.Add(new Models.Participation()
            {
                Id = Guid.NewGuid(),
                EvenementId = Evenement.Id,
                UtilisateurId = Evenement.OrganisateurId
            });
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Événement créé avec succès.";

                return RedirectToPage("/Index");
        }

        
            
        }
    }

