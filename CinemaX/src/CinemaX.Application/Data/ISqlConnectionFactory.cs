using System.Data;

namespace CinemaX.Application.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}