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

    }
}
