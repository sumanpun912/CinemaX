using System;
using System.Data;
using CinemaX.Application.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CinemaX.Infrastructure.Data;

public sealed class NpgsqlConnectionFactory : ISqlConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;
    
    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ??
                               throw new InvalidOperationException("Connection string 'Default' not configured.");
        _dataSource = NpgsqlDataSource.Create(connectionString);
    }

    public IDbConnection CreateConnection() => _dataSource.CreateConnection();
}