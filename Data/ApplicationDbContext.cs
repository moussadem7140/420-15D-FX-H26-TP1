using _420_15D_FX_H26_TP1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _420_15D_FX_H26_TP1.Data
{
    public class ApplicationDbContext : IdentityDbContext<Utilisateur>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Evenement> evenements { get; set; } // Représente la table "evenements"
        public DbSet<Categorie> Categories { get; set; } // Représente la table "Categories"
        public DbSet<Participation> Participations { get; set; } // Représente la table "Participations"
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1) Categorie (1) -> (N) Evenements
            modelBuilder.Entity<Evenement>()
                .HasOne(e => e.Categorie)
                .WithMany() // si tu as Categorie.Evenements, remplace par .WithMany(c => c.Evenements)
                .HasForeignKey(e => e.CategorieId)
                .OnDelete(DeleteBehavior.Restrict); // évite cascade multiple (tu peux mettre Cascade si tu veux)

            // 2) Utilisateur (1) -> (N) EvenementsOrganises (Organisateur)
            modelBuilder.Entity<Evenement>()
                .HasOne(e => e.Organisateur)
                .WithMany() // si tu as Utilisateur.EvenementsOrganises, remplace par .WithMany(u => u.EvenementsOrganises)
                .HasForeignKey(e => e.OrganisateurId)
                .OnDelete(DeleteBehavior.Restrict); // IMPORTANT

            // 3) Evenement (1) -> (N) Participations
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Evenement)
                .WithMany(e => e.Participants) // ici tu as List<Participation> Participants dans Evenement
                .HasForeignKey(p => p.EvenementId)
                .OnDelete(DeleteBehavior.Restrict); // IMPORTANT

            // 4) Utilisateur (1) -> (N) Participations
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Utilisateur)
                .WithMany() // si tu as Utilisateur.Participations, remplace par .WithMany(u => u.Participations)
                .HasForeignKey(p => p.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict); // IMPORTANT
        }

    }
}
