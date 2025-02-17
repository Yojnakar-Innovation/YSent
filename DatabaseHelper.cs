using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace YSent.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}