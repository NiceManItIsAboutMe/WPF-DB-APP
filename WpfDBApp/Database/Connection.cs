using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfDBApp.Database
{
    internal class Connection : IDisposable, IAsyncDisposable
    {
        private static readonly string _connectionString = "Host=localhost:5432; Username=postgres; " +
            "Password=postgres; Database=WpfDBApp";

        /*await using var cmd1 = new NpgsqlCommand("INSERT INTO public.users(name, surname, patronymic, description, superuser, login, password) VALUES('name','surname','patronymic','description', FALSE,$1,$2)", connection)
{ 
    Parameters =
    {
        new(){Value=Login},
        new(){Value=hash16}
    }
};
await cmd1.ExecuteNonQueryAsync();*/
        private NpgsqlConnection _connection;

        public Connection()
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        public NpgsqlConnection GetConnection()
        {
            _connection.Open();
            return _connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();

        }

        public ValueTask DisposeAsync()
        {
            return _connection.DisposeAsync();
        }
    }
}
