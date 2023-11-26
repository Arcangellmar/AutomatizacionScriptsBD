using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace AutomatizacionScriptsMysql.Interfaces
{
    public interface IDatabaseProvider
    {
        IDbConnection CreateConnection();
    }
    public class MySqlDatabaseProvider : IDatabaseProvider
    {
        private readonly string connectionString;

        public MySqlDatabaseProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }

    public class SqlServerDatabaseProvider : IDatabaseProvider
    {
        private readonly string connectionString;

        public SqlServerDatabaseProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
