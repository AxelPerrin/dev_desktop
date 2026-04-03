# Gestionnaire de Livres

Application de gestion de bibliothèque personnelle en .NET 8 / C#.

## Projets

- **GestionnaireLivres.WinForms** — CRUD complet, interface créée par code
- **GestionnaireLivres.WPF** — MVVM + SQLite + recherche + statistiques + export CSV
- **GestionnaireLivres.MAUI** — CollectionView, badges colorés par genre, filtre livres lus

## Lancer

```bash
# WinForms
cd GestionnaireLivres.WinForms
dotnet run

# WPF
cd GestionnaireLivres.WPF
dotnet run

# MAUI (Windows)
cd GestionnaireLivres.MAUI
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

## Bonus 1

````
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged;
}
