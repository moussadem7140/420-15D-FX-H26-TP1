namespace _420_15D_FX_H26_TP1.Models
{
    //Classe attendant la réponse de l'API resultaut pour récupérer les coordonnées d'une adresse
    public class ResultautApi
    {
        public List<Place> data { get; set; }
    }
    public class Place
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
