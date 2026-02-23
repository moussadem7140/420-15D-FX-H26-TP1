using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage ="Le code postal est requis")]
        [RegularExpression(@"^(?!.*[DFIOQUdfioqu])[A-VXYa-vxy]\d[A-Za-z]\s?\d[A-Za-z]\d$",
    ErrorMessage = "Le code postal doit être conforme aux normes du Canada.")]
        public string codePostal { get; set; }

        [Required(ErrorMessage = "La description est requise.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "La catégorie est requise.")]
        public Guid CategorieId { get; set; }

        public Categorie? Categorie;

        public string OrganisateurId { get; set; }
        public Utilisateur? Organisateur ;
        public List<Participation>? Participants { get; set; }

        [Required(ErrorMessage = "L'image de l'événement est requise.")]
        public string Image {  get; set; }
        public bool IsArchived {  get; set; } = false;

        // Propriété pour stocker la distance calculée, non mappée à la base de données
        [NotMapped]
        public double? Distance { get; set; }    


    }
}
