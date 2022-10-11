using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMVVMEfApp.Models.PostgreSqlDB
{
    //Диспетчер пакетов -> Add-Migration Initial -> Update-Database
    internal class ApplicationContext:DbContext
    {
        public DbSet<Book> books { get; set; }

        public DbSet<Category> categories { get; set; }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Author> Authors { get; set; }

        private readonly string _connectionStr = "Host=localhost;Port=5432;Database=Bookinist;Username=postgres;Password=postgres";

        public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options){ }
    }
}
