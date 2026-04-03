using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GestionnaireLivres.WPF.Data;
using GestionnaireLivres.WPF.Models;

namespace GestionnaireLivres.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly LivreRepository _repository;

    public MainViewModel()
    {
        _repository = new LivreRepository();
        Livres = new ObservableCollection<Livre>();
        Genres = new List<string> { "Roman", "SF", "Fantasy", "Policier", "Autre" };

        AjouterCommand = new RelayCommand(_ => Ajouter());
        ModifierCommand = new RelayCommand(_ => Modifier(), _ => LivreSelectionne != null);
        SupprimerCommand = new RelayCommand(_ => Supprimer(), _ => LivreSelectionne != null);
        ExporterCsvCommand = new RelayCommand(_ => ExporterCsv());

        ChargerLivres();
    }

    public ObservableCollection<Livre> Livres { get; }
    public List<string> Genres { get; }

    private Livre? _livreSelectionne;
    public Livre? LivreSelectionne
    {
        get => _livreSelectionne;
        set
        {
            if (SetProperty(ref _livreSelectionne, value) && value != null)
            {
                Titre = value.Titre;
                Auteur = value.Auteur;
                Annee = value.Annee;
                Genre = value.Genre;
                Lu = value.Lu;
            }
        }
    }

    private string _titre = "";
    public string Titre
    {
        get => _titre;
        set => SetProperty(ref _titre, value);
    }

    private string _auteur = "";
    public string Auteur
    {
        get => _auteur;
        set => SetProperty(ref _auteur, value);
    }

    private int _annee;
    public int Annee
    {
        get => _annee;
        set => SetProperty(ref _annee, value);
    }

    private string _genre = "Autre";
    public string Genre
    {
        get => _genre;
        set => SetProperty(ref _genre, value);
    }

    private bool _lu;
    public bool Lu
    {
        get => _lu;
        set => SetProperty(ref _lu, value);
    }

    private string _termeRecherche = "";
    public string TermeRecherche
    {
        get => _termeRecherche;
        set
        {
            if (SetProperty(ref _termeRecherche, value))
            {
                ChargerLivres();
            }
        }
    }

    private int _totalLivres;
    public int TotalLivres
    {
        get => _totalLivres;
        set => SetProperty(ref _totalLivres, value);
    }

    private int _livresLus;
    public int LivresLus
    {
        get => _livresLus;
        set => SetProperty(ref _livresLus, value);
    }

    private double _pourcentageLus;
    public double PourcentageLus
    {
        get => _pourcentageLus;
        set => SetProperty(ref _pourcentageLus, value);
    }

    public ICommand AjouterCommand { get; }
    public ICommand ModifierCommand { get; }
    public ICommand SupprimerCommand { get; }
    public ICommand ExporterCsvCommand { get; }

    private void ChargerLivres()
    {
        Livres.Clear();

        var liste = string.IsNullOrWhiteSpace(TermeRecherche)
            ? _repository.GetAll()
            : _repository.GetByRecherche(TermeRecherche);

        foreach (var livre in liste)
            Livres.Add(livre);

        MettreAJourStatistiques();
    }

    private void MettreAJourStatistiques()
    {
        TotalLivres = Livres.Count;
        LivresLus = Livres.Count(l => l.Lu);
        PourcentageLus = TotalLivres > 0 ? Math.Round((double)LivresLus / TotalLivres * 100, 1) : 0;
    }

    private bool ValiderFormulaire(out string erreurs)
    {
        var listeErreurs = new List<string>();

        if (string.IsNullOrWhiteSpace(Titre) || Titre.Trim().Length < 2)
            listeErreurs.Add("• Titre : obligatoire, minimum 2 caractères");

        if (string.IsNullOrWhiteSpace(Auteur) || Auteur.Trim().Length < 2)
            listeErreurs.Add("• Auteur : obligatoire, minimum 2 caractères");

        if (Annee < 1800 || Annee > DateTime.Now.Year)
            listeErreurs.Add($"• Année : doit être entre 1800 et {DateTime.Now.Year}");

        if (string.IsNullOrWhiteSpace(Genre))
            listeErreurs.Add("• Genre : vous devez sélectionner un genre");

        erreurs = string.Join("\n", listeErreurs);
        return listeErreurs.Count == 0;
    }

    private void Ajouter()
    {
        if (!ValiderFormulaire(out string erreurs))
        {
            MessageBox.Show(erreurs, "Erreurs de validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var livre = new Livre
        {
            Titre = Titre.Trim(),
            Auteur = Auteur.Trim(),
            Annee = Annee,
            Genre = Genre,
            Lu = Lu
        };

        _repository.Add(livre);
        ViderFormulaire();
        ChargerLivres();
    }

    private void Modifier()
    {
        if (LivreSelectionne == null) return;

        if (!ValiderFormulaire(out string erreurs))
        {
            MessageBox.Show(erreurs, "Erreurs de validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        LivreSelectionne.Titre = Titre.Trim();
        LivreSelectionne.Auteur = Auteur.Trim();
        LivreSelectionne.Annee = Annee;
        LivreSelectionne.Genre = Genre;
        LivreSelectionne.Lu = Lu;

        _repository.Update(LivreSelectionne);
        ViderFormulaire();
        ChargerLivres();
    }

    private void Supprimer()
    {
        if (LivreSelectionne == null) return;

        var result = MessageBox.Show(
            $"Voulez-vous vraiment supprimer \"{LivreSelectionne.Titre}\" ?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _repository.Delete(LivreSelectionne.Id);
            ViderFormulaire();
            ChargerLivres();
        }
    }

    private void ViderFormulaire()
    {
        Titre = "";
        Auteur = "";
        Annee = 0;
        Genre = "Autre";
        Lu = false;
        LivreSelectionne = null;
    }

    private void ExporterCsv()
    {
        var allLivres = _repository.GetAll();
        var chemin = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "livres.csv");

        var lignes = new List<string> { "Titre;Auteur;Annee;Genre;Lu" };
        foreach (var l in allLivres)
            lignes.Add($"{l.Titre};{l.Auteur};{l.Annee};{l.Genre};{(l.Lu ? "Oui" : "Non")}");

        File.WriteAllLines(chemin, lignes);
        MessageBox.Show($"Export réussi !\n{chemin}", "Export CSV", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
