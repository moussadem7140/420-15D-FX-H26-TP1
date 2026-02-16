namespace _420_15D_FX_H26_TP1.Models
{
    public class Participation
    {
        public Guid Id { get; set; }
        public Guid EvenementId { get; set; }
        public Evenement Evenement { get; set; }
        public string UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }
}
