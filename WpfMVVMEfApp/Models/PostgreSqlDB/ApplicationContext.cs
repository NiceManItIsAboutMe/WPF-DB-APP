using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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
        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Author> Authors { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login).IsUnique(true);
            modelBuilder.Entity<Category>()
                .HasIndex(u => u.Name).IsUnique(true);
            // сочетание фамилии имени и отчетсва должны быть уникальными
            modelBuilder.Entity<Author>()
                .HasIndex(u => new { u.Surname, u.Name, u.Patronymic }).IsUnique(true);
            base.OnModelCreating(modelBuilder);

        }
    }

    /// <summary>
    /// Для создания миграций
    /// </summary>
    internal class YourDbContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Bookinist;Username=postgres;Password=postgres");

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
