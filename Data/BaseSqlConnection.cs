using Npgsql;

namespace shopWebAPI.Data;

public abstract class BaseSqlConnection
{
    
    protected readonly NpgsqlConnection _connection;
    protected string sql = string.Empty;
    protected BaseSqlConnection(string? connectionString)
    {
        
        _connection = new NpgsqlConnection(connectionString);
    }
    
}