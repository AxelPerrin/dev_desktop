namespace GestionnaireLivres.WinForms;

public class Livre
{
    public string Titre { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public int Annee { get; set; }
    public string Genre { get; set; } = string.Empty;
    public bool Lu { get; set; }

    public override string ToString()
    {
        string luStr = Lu ? "✓" : "✗";
        return $"{Titre} — {Auteur} ({Annee}) [{Genre}] Lu: {luStr}";
    }
}
