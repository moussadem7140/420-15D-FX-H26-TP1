using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using _420_15D_FX_H26_TP1.Models;

namespace _420_15D_FX_H26_TP1.Data
{
    public static class DbInitializer
    {
        private const string ROLE_ADMIN = "Admin";
        private const string ROLE_USER = "Utilisateur";
        private const string DEFAULT_PASSWORD = "Password123!";

        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Assure DB à jour (migrations)
            await context.Database.MigrateAsync();

            // 1) Rôles
            await EnsureRoleAsync(roleManager, ROLE_ADMIN);
            await EnsureRoleAsync(roleManager, ROLE_USER);

            // 2) Utilisateurs (min 5)
            var users = new List<Utilisateur>();

            // Admin obligatoire
            var admin = await EnsureUserAsync(
                userManager,
                username: "admin",
                email: "admin@cegepgarneau.ca",
                nom: "Admin",
                prenom: "Admin",
                ville: "Québec",
                adresse: "1660 Boulevard de l'Entente",
                codePostal: "G1V 0A6",
                password: DEFAULT_PASSWORD);

            await EnsureUserInRoleAsync(userManager, admin, ROLE_ADMIN);
            users.Add(admin);

            // 4 autres utilisateurs (au total 5)
            users.Add(await EnsureStudentUserAsync(userManager, "moussa", "moussa@cegepgarneau.ca", "Dembélé", "Moussa"));
            users.Add(await EnsureStudentUserAsync(userManager, "sara", "sara@cegepgarneau.ca", "Traoré", "Sara"));
            users.Add(await EnsureStudentUserAsync(userManager, "adam", "adam@cegepgarneau.ca", "Diallo", "Adam"));
            users.Add(await EnsureStudentUserAsync(userManager, "ina", "ina@cegepgarneau.ca", "Koné", "Ina"));

            foreach (var u in users.Where(x => x.Id != admin.Id))
                await EnsureUserInRoleAsync(userManager, u, ROLE_USER);

            // 3) Catégories (min 5)
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Categorie>
                {
                    new() { Id = Guid.NewGuid(), Nom = "Sport",        IsArchived = false },
                    new() { Id = Guid.NewGuid(), Nom = "Musique",      IsArchived = false },
                    new() { Id = Guid.NewGuid(), Nom = "Technologie",  IsArchived = false },
                    new() { Id = Guid.NewGuid(), Nom = "Éducation",    IsArchived = false },
                    new() { Id = Guid.NewGuid(), Nom = "Culture",      IsArchived = false },
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            var categorieList = await context.Categories
                .Where(c => !c.IsArchived)
                .ToListAsync();

            // 4) Évènements (min 2 par catégorie, organisateurs aléatoires)
            if (!await context.evenements.AnyAsync())
            {
                var rnd = new Random();
                var events = new List<Evenement>();

                foreach (var cat in categorieList)
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        var org = users[rnd.Next(users.Count)];
                        var start = DateTime.Today
                            .AddDays(rnd.Next(1, 45 - i))
                            .AddHours(rnd.Next(8, 19 - i));
                        var end = start.AddHours(rnd.Next(2, 6 - i));

                        events.Add(new Evenement
                        {
                            Id = Guid.NewGuid(),
                            Nom = $"{cat.Nom} - Évènement {i}",
                            DateDebut = start,
                            DateFin = end,
                            Ville = "Québec",
                            Adresse = $"{rnd.Next(10, 999)} Rue Saint-Jean",
                            codePostal = "G1R 1N6", // format postal CA valide
                            Description = $"Activité {cat.Nom.ToLower()} (seed).",
                            CategorieId = cat.Id,
                            OrganisateurId = org.Id,     // FK vers Utilisateur.Id
                            Image = "parDefaut.jpg",
                            IsArchived = false
                        });
                    }
                }

                await context.evenements.AddRangeAsync(events);
                await context.SaveChangesAsync();

                // >>> ICI : l’organisateur devient automatiquement participant <<<
                var organiserParticipations = events.Select(ev => new Participation
                {
                    Id = Guid.NewGuid(),
                    UtilisateurId = ev.OrganisateurId, // même Id que l’organisateur
                    EvenementId = ev.Id
                }).ToList();

                await context.Participations.AddRangeAsync(organiserParticipations);
                await context.SaveChangesAsync();
            }

            // 5) Participations (chaque user participe à au moins 2 évènements)
            //    IMPORTANT : évite les doublons (même user + même event)
            var allEvents = await context.evenements
                .Where(e => !e.IsArchived)
                .Select(e => e.Id)
                .ToListAsync();

            if (allEvents.Count >= 2)
            {
                var rnd = new Random();

                // On charge les participations existantes (y compris celles des organisateurs)
                var existing = await context.Participations
                    .Select(p => new { p.UtilisateurId, p.EvenementId })
                    .ToListAsync();

                foreach (var u in users)
                {
                    int already = existing.Count(x => x.UtilisateurId == u.Id);

                    int missing = Math.Max(0, 2 - already);
                    if (missing == 0) continue;

                    var picked = new HashSet<Guid>();
                    for (int k = 0; k < 20 && picked.Count < missing; k++)
                    {
                        var evId = allEvents[rnd.Next(allEvents.Count)];

                        // pas de doublon
                        if (existing.Any(x => x.UtilisateurId == u.Id && x.EvenementId == evId))
                            continue;
                        if (!picked.Add(evId))
                            continue;
                    }

                    var parts = picked.Select(evId => new Participation
                    {
                        Id = Guid.NewGuid(),
                        UtilisateurId = u.Id,
                        EvenementId = evId
                    });

                    await context.Participations.AddRangeAsync(parts);
                    await context.SaveChangesAsync();

                    // met à jour cache local
                    existing.AddRange(picked.Select(evId => new { UtilisateurId = u.Id, EvenementId = evId }));
                }
            }
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        private static async Task<Utilisateur> EnsureStudentUserAsync(
            UserManager<Utilisateur> userManager,
            string username,
            string email,
            string nom,
            string prenom)
        {
            var u = await EnsureUserAsync(
                userManager,
                username: username,
                email: email,
                nom: nom,
                prenom: prenom,
                ville: "Québec",
                adresse: "1660 Boulevard de l'Entente",
                codePostal: "G1V 0A6",
                password: DEFAULT_PASSWORD);

            return u;
        }

        private static async Task<Utilisateur> EnsureUserAsync(
            UserManager<Utilisateur> userManager,
            string username,
            string email,
            string nom,
            string prenom,
            string ville,
            string adresse,
            string codePostal,
            string password)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
                return existing;

            var user = new Utilisateur
            {
                UserName = username,
                Email = email,
                Nom = nom,
                Prenom = prenom,
                Ville = ville,
                Adresse = adresse,
                codePostal = codePostal,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var msg = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new Exception("Seed user creation failed: " + msg);
            }

            return user;
        }

        private static async Task EnsureUserInRoleAsync(
            UserManager<Utilisateur> userManager,
            Utilisateur user,
            string role)
        {
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
    }
}