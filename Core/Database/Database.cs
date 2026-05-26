using Microsoft.Data.Sqlite;
using System.ComponentModel;
using System.Data;

public sealed class Database : IDisposable
{
    private readonly SqliteConnection _connection;

    public Database(string path = "app.db")
    {
        _connection = new SqliteConnection($"Data Source={path}");
        _connection.Open();
        EnableWAL();
        
    }

    public IDbConnection Connection => _connection;

    private void EnableWAL()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "PRAGMA journal_mode=WAL;";
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => _connection.Dispose();

}