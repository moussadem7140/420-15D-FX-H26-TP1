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
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        //Le supportsGet vaa me permettre d'utiliser une variable obtenu avec get dans ma methode
        //Je voulais faire la mathode en Get pour respecter la différence entre Post et Get et ne pas faire du Post pour une recherche 
        [BindProperty(SupportsGet =true)]
        public string motCle {  get; set; }

        //contructeur injectant mes services
        public IndexModel(_420_15D_FX_H26_TP1.Data.ApplicationDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }
        
        public List<Evenement> Evenements { get;set; }

        public SelectList Categories { get; set; }

        [BindProperty(SupportsGet =true)]
        public Guid? CategorieCle { get; set; }

        //cette methode va gérer aussi bien l'affichage de tous les évenements que l'affichage des évenements recherchés par mot clé ou catégorie ou les deux
        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(motCle) || CategorieCle != null)
            {
                //Cas ou l'utilisateur a entré un mot clé ou choisi une catégorie ou les deux pour faire une recherche
                ViewData["titre"] = "Les Evenements Recherchés";
                //mocle est nulle ou vide et categorie nulle
                if (CategorieCle == null)
                {
                    Evenements = await _context.evenements
                  .Include(e => e.Categorie)
                  .Include(e => e.Organisateur)
                  .Include(e => e.Participants)
                  .ThenInclude(p => p.Utilisateur)
                  .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && (e.Nom.Contains(motCle.Trim()) || e.Description.Contains(motCle.Trim())))
                  .OrderBy(e => e.DateDebut)
                  .ToListAsync();
                    Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                    if (User.Identity.IsAuthenticated)
                    {
                        Evenements = await TrierParAdresse(Evenements);
                    }

                }
                //categorie nulle ou mot clé null ou vide
                else if (string.IsNullOrEmpty(motCle))
                {
                    Evenements = await _context.evenements
                  .Include(e => e.Categorie)
                  .Include(e => e.Organisateur)
                  .Include(e => e.Participants)
                  .ThenInclude(p => p.Utilisateur)
                  .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && e.CategorieId == CategorieCle)
                  .OrderBy(e => e.DateDebut)
                  .ToListAsync();
                    Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");

                    if (User.Identity.IsAuthenticated)
                    {
                        Evenements = await TrierParAdresse(Evenements);
                    }
                }
                //ni mot clé ni catégorie nulle    
                else
                {
                   Evenements = await _context.evenements
                 .Include(e => e.Categorie)
                 .Include(e => e.Organisateur)
                 .Include(e => e.Participants)
                 .ThenInclude(p => p.Utilisateur)
                 .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && (e.Nom.Contains(motCle.Trim()) || e.Description.Contains(motCle.Trim())) && e.CategorieId == CategorieCle)
                 .OrderBy(e => e.DateDebut)
                 .ToListAsync();
                 Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                    if (User.Identity.IsAuthenticated)
                    {
                        ViewData["titre"] = "Les Evenements les plus proches de chez vous";
                        Evenements = await TrierParAdresse(Evenements);
                    }
                }
     
            }
            else
            {
                //Pas de Recherche, affichage de tous les évenements
                ViewData["titre"] = "Les Evenements";
                Evenements = await _context.evenements
               .Include(e => e.Categorie)
               .Include(e => e.Organisateur)
               .Include(e => e.Participants)
               .ThenInclude(p => p.Utilisateur)
               .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false)
               .OrderBy(e => e.DateDebut)
               .ToListAsync();
                Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
                if (User.Identity.IsAuthenticated)
                {
                    ViewData["titre"] = "Les Evenements les plus proches de chez vous";
                    Evenements = await TrierParAdresse(Evenements);
                }

            }
           
        }
      
        public async Task OnGetParticipations()
        {
            //Cette méthode va me permettre d'afficher les évenements auxquels l'utilisateur connecté participe
            Evenements = await _context.evenements
               .Include(e => e.Categorie)
               .Include(e => e.Organisateur)
               .Include(e => e.Participants)
               .ThenInclude(p => p.Utilisateur)
               .Where(e => e.DateDebut >= DateTime.Now && e.IsArchived == false && (e.Participants.Any(p=> p.Utilisateur.UserName== User.Identity.Name))&& e.Organisateur.UserName != User.Identity.Name)
               .OrderBy(e => e.DateDebut)
               .ToListAsync();
            ViewData["titre"] = "La liste de mes participations";
            Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
            Evenements = await TrierParAdresse(Evenements);
            //Je n'ai pas mis le tri par adresse pour les évenements
            //que j'organise ou que je participe parce que je trouve que c'est moins pertinent(Economie de requete API)
        }
        public async Task OnGetMesEvenements()
        {
            //Cette méthode va me permettre d'afficher les évenements que l'utilisateur connecté organise
            //mais va pouvoir afficher autant les evenements passés que les évenements archivés pour que l'utilisateur puisse avoir un historique de tous les évenements qu'il a organisé
            Evenements = await _context.evenements
               .Include(e => e.Categorie)
               .Include(e => e.Organisateur)
               .Include(e => e.Participants)
               .ThenInclude(p => p.Utilisateur)
               .Where(e=>e.Organisateur.UserName==User.Identity.Name)
               .OrderBy(e => e.DateDebut)
               .ToListAsync();
            ViewData["titre"] = "Historique de mes évenements";
            Categories = new SelectList(_context.Categories.Where(e => !e.IsArchived), "Id", "Nom");
            //Je n'ai pas mis le tri par adresse pour les évenements
            //que j'organise ou que je participe parce que je trouve que c'est moins pertinent(Economie de requete API)
        }
        private async Task< List<Evenement>> TrierParAdresse(List<Evenement> evenements)
        {
            //Cette méthode va me permettre de trier les évenements par distance par rapport à l'utilisateur connecté
            string codePostalUtilisateur = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).codePostal;
            string Url = $"http://api.positionstack.com/v1/forward?access_key={_configuration["ApiKey"]}&query={codePostalUtilisateur}&country=CA&limit=1";
            ResultautApi result = await _httpClient.GetFromJsonAsync<ResultautApi>(Url);
            Place placeUtilisateur = result.data[0];
            foreach (var evenement in evenements)
            {
                string UrlEvenement =  $"http://api.positionstack.com/v1/forward?access_key={_configuration["ApiKey"]}&query={evenement.codePostal}&country=CA&limit=1";
                await Task.Delay(1100); // Je le mets pour éviter de dépasser la limite de requete de l'API (1 requete par seconde)
                                        // parce que je suis en mode bAsic et que ca avait tendance à planter
                ResultautApi resultEvenement = await _httpClient.GetFromJsonAsync<ResultautApi>(UrlEvenement);
                Place placeEvenement = resultEvenement.data[0];
                double distance = CaculateurDeDistance(placeUtilisateur.latitude, placeUtilisateur.longitude, placeEvenement.latitude, placeEvenement.longitude);
                evenement.Distance = distance;

            }
            return evenements.OrderBy(e => e.Distance)
                .ThenBy(e => e.DateDebut)
                .ToList();
        }
        //elle va calculer la distance entre deux points géographiques à partir de leurs latitudes et longitudes en utilisant la formule de Haversine(Formule Vu sur Youtube honnêtement)
        private static double CaculateurDeDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // rayon de la Terre en km
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double DegreesToRadians(double deg)
            => deg * (Math.PI / 180.0);

    }
}
