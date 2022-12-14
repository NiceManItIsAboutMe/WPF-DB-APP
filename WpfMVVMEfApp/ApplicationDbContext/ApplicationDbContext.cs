using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WpfMVVMEfApp.Models.PostgreSqlDB
{
    //Диспетчер пакетов -> Add-Migration Initial -> Update-Database
    internal class ApplicationDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<BookFileDescription> BookFilesDescription { get; set; }

        public DbSet<BookFile> BookFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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
    internal class YourDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Bookinist;Username=postgres;Password=postgres");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
