namespace _420_15D_FX_H26_TP1.Models
{
    public class Evenement
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Ville { get; set; }
        public string Adresse { get; set; }
        public string codePostal { get; set; }
        public string Description { get; set; }
        public Guid CategorieId { get; set; }
        public Categorie Categorie { get; set; }
        public string OrganisateurId { get; set; }
        public Utilisateur Organisateur { get; set; }
        public List<Participation> Participants { get; set; }
        public string Image { get; set; }
        public bool IsArchived {  get; set; }
    }
}
