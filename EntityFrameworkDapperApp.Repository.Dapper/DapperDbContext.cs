using System.Data;
using Npgsql;

namespace EntityFrameworkDapperApp.Repository.Dapper;

public class DapperDbContext
{
    private readonly string _dbConnection;
    private readonly string _dbConnectionMaster;
    private readonly string _applicationDbName;

    public DapperDbContext(string connectionString)
    {
        _dbConnection = connectionString;
        _dbConnectionMaster = GetMasterDbConnection(_dbConnection);
        _applicationDbName = GetDatabaseName(_dbConnection);
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_dbConnection);

    public IDbConnection CreateConnectionMaster()
        => new NpgsqlConnection(_dbConnectionMaster);
    
    private string GetDatabaseName(string connectionString)
    {
        return connectionString
            .Split(';')
            .SingleOrDefault(x => x.ToLower().Contains("database="))
            ?.Split('=')
            .ElementAt(1) ?? string.Empty;
    }

    private string GetMasterDbConnection(string connectionString)
    {
        var dbConnection = connectionString.Split(';').Where(x => !x.ToLower().Contains("database="));
        return string.Join(';', dbConnection);
    }
}