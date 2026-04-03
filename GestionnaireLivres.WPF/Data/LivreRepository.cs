using Microsoft.Data.Sqlite;
using GestionnaireLivres.WPF.Models;

namespace GestionnaireLivres.WPF.Data;

public class LivreRepository
{
    private readonly string _connectionString;

    public LivreRepository(string dbPath = "livres.db")
    {
        _connectionString = $"Data Source={dbPath}";
        InitialiserBase();
    }

    public void InitialiserBase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS livres (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                titre TEXT NOT NULL,
                auteur TEXT NOT NULL,
                annee INTEGER,
                genre TEXT NOT NULL DEFAULT 'Autre',
                lu INTEGER NOT NULL DEFAULT 0
            )";
        command.ExecuteNonQuery();
    }

    public List<Livre> GetAll()
    {
        var livres = new List<Livre>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, titre, auteur, annee, genre, lu FROM livres";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            livres.Add(new Livre
            {
                Id = reader.GetInt32(0),
                Titre = reader.GetString(1),
                Auteur = reader.GetString(2),
                Annee = reader.GetInt32(3),
                Genre = reader.GetString(4),
                Lu = reader.GetInt32(5) == 1
            });
        }

        return livres;
    }

    public List<Livre> GetByRecherche(string terme)
    {
        var livres = new List<Livre>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, titre, auteur, annee, genre, lu FROM livres
            WHERE titre LIKE @terme OR auteur LIKE @terme";
        command.Parameters.AddWithValue("@terme", $"%{terme}%");

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            livres.Add(new Livre
            {
                Id = reader.GetInt32(0),
                Titre = reader.GetString(1),
                Auteur = reader.GetString(2),
                Annee = reader.GetInt32(3),
                Genre = reader.GetString(4),
                Lu = reader.GetInt32(5) == 1
            });
        }

        return livres;
    }

    public void Add(Livre livre)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO livres (titre, auteur, annee, genre, lu)
            VALUES (@titre, @auteur, @annee, @genre, @lu)";
        command.Parameters.AddWithValue("@titre", livre.Titre);
        command.Parameters.AddWithValue("@auteur", livre.Auteur);
        command.Parameters.AddWithValue("@annee", livre.Annee);
        command.Parameters.AddWithValue("@genre", livre.Genre);
        command.Parameters.AddWithValue("@lu", livre.Lu ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public void Update(Livre livre)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE livres SET titre = @titre, auteur = @auteur, annee = @annee,
            genre = @genre, lu = @lu WHERE id = @id";
        command.Parameters.AddWithValue("@id", livre.Id);
        command.Parameters.AddWithValue("@titre", livre.Titre);
        command.Parameters.AddWithValue("@auteur", livre.Auteur);
        command.Parameters.AddWithValue("@annee", livre.Annee);
        command.Parameters.AddWithValue("@genre", livre.Genre);
        command.Parameters.AddWithValue("@lu", livre.Lu ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM livres WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }
}
