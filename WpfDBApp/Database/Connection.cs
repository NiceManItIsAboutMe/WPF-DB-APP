using Microsoft.VisualBasic;
using Npgsql;
using System.Threading.Tasks;

namespace WpfDBApp.Database
{
    internal static class Connection
    {
        private static readonly string _connectionString = "Host=localhost:5432; Username=postgres; Password=postgres; Database=WpfDBApp";

        public static async ValueTask<NpgsqlConnection> GetConnection()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
