using System.ComponentModel.DataAnnotations;

namespace _420_15D_FX_H26_TP1.Models
{
    public class Evenement
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Le nom de l'événement est requis.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "La date de début est requise.")]
        [DataType(DataType.Date, ErrorMessage = "Format de date invalide.")]

        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de fin est requise.")]
        [DataType(DataType.Date, ErrorMessage = "Format de date invalide.")]
        public DateTime DateFin { get; set; }

        [Required(ErrorMessage = "La ville est requise.")]
        public string Ville { get; set; }
        [Required(ErrorMessage = "L'adresse est requise.")]
        public string Adresse { get; set; }
        [Required(ErrorMessage = "Le code postal est requis.")]
        [RegularExpression(@"^(?!.*[DFIOQUdfioqu])[A-VXYa-vxy][0-9][A-Za-z]\s?[0-9][A-Za-z][0-9]$
        ", ErrorMessage = "Le code postal doit être conforme au normes du canada (Voir sur Internet) .")]
        public string codePostal { get; set; }
        [Required(ErrorMessage = "La description est requise.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "La catégorie est requise.")]
        public Guid CategorieId { get; set; }

        public Categorie Categorie { get; set; }

        public string OrganisateurId { get; set; }
        public Utilisateur Organisateur { get; set; }
        public List<Participation> Participants { get; set; }= new List<Participation>();
        [Required(ErrorMessage = "L'image est requise.")]
        public string Image { get; set; }
        public bool IsArchived {  get; set; }
    }
}
