using Dapper;
using System.ComponentModel;
using System.Data;

public sealed class Repository(Database db)
{
    private IDbConnection Conn => db.Connection;


    public IEnumerable<T> Query<T>(string sql, object? param = null)
        => Conn.Query<T>(sql, param);

    public T? QuerySingle<T>(string sql, object? param = null)
        => Conn.QuerySingleOrDefault<T>(sql, param);

    public int Execute(string sql, object? param = null)
        => Conn.Execute(sql, param);
}