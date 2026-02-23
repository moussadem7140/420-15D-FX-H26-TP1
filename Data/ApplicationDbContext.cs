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
                .WithMany() // 
                .HasForeignKey(e => e.CategorieId)
                .OnDelete(DeleteBehavior.Restrict); 

            // 2) Utilisateur (1) -> (N) EvenementsOrganises (Organisateur)
            modelBuilder.Entity<Evenement>()
                .HasOne(e => e.Organisateur)
                .WithMany() 
                .HasForeignKey(e => e.OrganisateurId)
                .OnDelete(DeleteBehavior.Restrict); 
            // 3) Evenement (1) -> (N) Participations
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Evenement)
                .WithMany(e => e.Participants) 
                .HasForeignKey(p => p.EvenementId)
                .OnDelete(DeleteBehavior.Restrict); 

            // 4) Utilisateur (1) -> (N) Participations
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Utilisateur)
                .WithMany() 
                .HasForeignKey(p => p.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict); 
        }

    }
}
