using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace WpfMVVMEfApp.Models.PostgreSqlDB
{
    internal class DBInitializer
    {

        private readonly ApplicationContext _db;
        private readonly ILogger<DBInitializer> _logger;
        private Category[] _Categories;
        private Author[] _Authors;
        private Book[] _Books;
        public DBInitializer(ApplicationContext db, ILogger<DBInitializer> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            var timer = Stopwatch.StartNew();
            _logger.LogInformation("------------------------Инициализация БД------------------------");

            _logger.LogInformation("Удаление БД");
            await _db.Database.EnsureDeletedAsync();
            _logger.LogInformation("Удаление БД выполнено спустя {0} мс", timer.ElapsedMilliseconds);

            _logger.LogInformation("Миграция БД");
            await _db.Database.MigrateAsync();
            _logger.LogInformation("Миграция БД выполнено спустя {0} мс", timer.ElapsedMilliseconds);

            if (await _db.books.AnyAsync()) return;

            Random random = new Random();
            _Categories = Enumerable
                .Range(1, 5)
                .Select(i => new Category { Name = $"Категория {i}" })
                .ToArray();

            _Authors = Enumerable
                .Range(1, 5)
                .Select(i => new Author { Name = $"Автор {i}" })
                .ToArray();

            _Books = Enumerable
               .Range(1, 500)
               .Select(i => new Book 
               { 
                   Name = $"Книга {i}",
                   Category = random.NextItem<Category>(_Categories),
                   Author = random.NextItem<Author>(_Authors)
               })
               .ToArray();
            await _db.AddRangeAsync(_Categories);
            await _db.AddRangeAsync(_Authors);
            await _db.AddRangeAsync(_Books);
            await _db.SaveChangesAsync();

        }
    }
}
