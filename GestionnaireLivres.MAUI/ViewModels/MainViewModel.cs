using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GestionnaireLivres.MAUI.Models;

namespace GestionnaireLivres.MAUI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private ObservableCollection<Livre> _tousLesLivres = new();
    public ObservableCollection<Livre> LivresFiltres { get; } = new();

    private bool _afficherLusUniquement;
    public bool AfficherLusUniquement
    {
        get => _afficherLusUniquement;
        set
        {
            if (_afficherLusUniquement != value)
            {
                _afficherLusUniquement = value;
                OnPropertyChanged();
                AppliquerFiltre();
            }
        }
    }

    private Livre? _livreSelectionne;
    public Livre? LivreSelectionne
    {
        get => _livreSelectionne;
        set
        {
            if (_livreSelectionne != value)
            {
                _livreSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                    AfficherDetail(value);
            }
        }
    }

    public MainViewModel()
    {
        _tousLesLivres = new ObservableCollection<Livre>
        {
            new Livre { Id = 1, Titre = "jjjjjj", Auteur = "kkkkkkk", Annee = 1855, Genre = "Roman", Lu = true },
            new Livre { Id = 2, Titre = "lllllll", Auteur = "vvvvvv", Annee = 1988, Genre = "SF", Lu = true },
            new Livre { Id = 3, Titre = "aaaaaaa", Auteur = "ffffff", Annee = 2000, Genre = "Fantasy", Lu = false },
            new Livre { Id = 4, Titre = "nnnnnnn", Auteur = "xxxxx", Annee = 1890, Genre = "Policier", Lu = false },
            new Livre { Id = 5, Titre = "cccccc", Auteur = "vvvvvvv", Annee = 2010, Genre = "Autre", Lu = true }
        };

        AppliquerFiltre();
    }

    private void AppliquerFiltre()
    {
        LivresFiltres.Clear();

        var source = AfficherLusUniquement
            ? _tousLesLivres.Where(l => l.Lu)
            : _tousLesLivres;

        foreach (var livre in source)
            LivresFiltres.Add(livre);
    }

    private async void AfficherDetail(Livre livre)
    {
        string luStr = livre.Lu ? "Oui" : "Non";
        string message = $"Auteur : {livre.Auteur}\n" +
                         $"Année : {livre.Annee}\n" +
                         $"Genre : {livre.Genre}\n" +
                         $"Lu : {luStr}";

        if (Application.Current?.Windows.Count > 0)
        {
            var page = Application.Current.Windows[0].Page;
            if (page != null)
            {
                await page.DisplayAlert(livre.Titre, message, "OK");
                LivreSelectionne = null;
            }
        }
    }
}
