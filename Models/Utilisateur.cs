using Microsoft.AspNetCore.Identity;

namespace _420_15D_FX_H26_TP1.Models
{
    public class Utilisateur: IdentityUser
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Ville { get; set; }
        public string Adresse { get; set; }
        public string codePostal { get; set; }
    }
}
