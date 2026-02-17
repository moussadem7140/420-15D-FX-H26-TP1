using System.ComponentModel.DataAnnotations;

namespace _420_15D_FX_H26_TP1.Models
{
    public class Categorie
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Le nom de la catégorie est requis.")]
        public string Nom { get; set; }
        public bool IsArchived { get; set; }    
    }
}
