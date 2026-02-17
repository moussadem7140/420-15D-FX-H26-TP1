using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _420_15D_FX_H26_TP1.Models
{
    public class Utilisateur: IdentityUser
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "La ville est requise.")]
        public string Ville { get; set; }
        [Required(ErrorMessage = "L'adresse est requise.")]
        public string Adresse { get; set; }
        [Required(ErrorMessage = "Le code postal est requis.")]
        [RegularExpression(@"^(?!.*[DFIOQUdfioqu])[A-VXYa-vxy][0-9][A-Za-z]\s?[0-9][A-Za-z][0-9]$
        ", ErrorMessage = "Le code postal doit être conforme au normes du canada (Voir sur Internet) .")]
        public string codePostal { get; set; }
    }
}
