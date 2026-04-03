namespace GestionnaireLivres.MAUI.Models;

public class Livre
{
    public int Id { get; set; }
    public string Titre { get; set; } = "";
    public string Auteur { get; set; } = "";
    public int Annee { get; set; }
    public string Genre { get; set; } = "Autre";
    public bool Lu { get; set; }
}
