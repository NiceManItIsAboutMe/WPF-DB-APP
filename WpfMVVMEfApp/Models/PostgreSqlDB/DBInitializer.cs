using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfMVVMEfApp.Models.PostgreSqlDB
{
    internal class DBInitializer
    {

        private readonly ApplicationContext _db;
        private readonly ILogger<DBInitializer> _logger;
        private Category[] _Categories;
        private Author[] _Authors;
        private Book[] _Books;
        private User[] _Users;
        public DBInitializer(ApplicationContext db, ILogger<DBInitializer> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {

                var timer = Stopwatch.StartNew();
                _logger.LogInformation("------------------------Инициализация БД------------------------");

                //_logger.LogInformation("Удаление БД");
                await _db.Database.EnsureDeletedAsync();
                //_logger.LogInformation("Удаление БД выполнено спустя {0} мс", timer.ElapsedMilliseconds);


                _logger.LogInformation("Миграция БД");

                if (_db.Database.IsInMemory())
                    await _db.Database.EnsureCreatedAsync();
                else
                    await _db.Database.MigrateAsync();

                _logger.LogInformation("Миграция БД выполнено спустя {0} мс", timer.ElapsedMilliseconds);

                // если в базе уже что-то есть не инициализируем
                if (await _db.Books.AnyAsync()) { _logger.LogInformation("База данных существует и полна"); return; }

                Random random = new Random();
                _Categories = Enumerable
                    .Range(1, 5)
                    .Select(i => new Category { Name = $"Категория {i}" }).ToArray();

                _Authors = Enumerable
                    .Range(1, 5)
                    .Select(i => new Author
                    {
                        Surname = $"Фамилия {i}",
                        Name = $"Автор {i}",
                        Patronymic = $"Отчество {i}",
                    }).ToArray();

                _Books = Enumerable
                   .Range(1, 500)
                   .Select(i => new Book
                   {
                       Name = $"Книга {i}",
                       Description =$"Описание книги {i}...",
                       Category = _Categories.ToList().GetRange(random.Next(0,4), 2),
                       Author = random.NextItem<Author>(_Authors)
                   }).ToArray();
                _Users = Enumerable
                   .Range(1, 5)
                   .Select(i => new User
                   {
                       Surname = $"Фамилия {i}",
                       Name = $"Имя {i}",
                       Patronymic = $"Отчество {i}",
                       Birthday = DateOnly.FromDateTime(DateTime.Now),
                       Login = $"login{i}",
                       Password = User.HashPassword($"password{i}"),
                       IsAdmin = false,
                       Books= _Books.ToList().GetRange(random.Next(1,490),10)
                   }).ToArray();

                await _db.AddRangeAsync(_Categories);
                await _db.AddRangeAsync(_Authors);
                await _db.AddRangeAsync(_Users);
                await _db.AddRangeAsync(_Books);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Инициализация завершена {0} мс", timer.ElapsedMilliseconds);
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Нет доступа к базе данных. Обратитесь в службу поддержки" + Environment.NewLine + ex.Message);
            }
        }

        public void Initialize()
        {
            try
            {

                var timer = Stopwatch.StartNew();
                _logger.LogInformation("------------------------Инициализация БД------------------------");

                //_logger.LogInformation("Удаление БД");
                _db.Database.EnsureDeleted();
                //_logger.LogInformation("Удаление БД выполнено спустя {0} мс", timer.ElapsedMilliseconds);


                _logger.LogInformation("Миграция БД");

                if (_db.Database.IsInMemory())
                    _db.Database.EnsureCreated();
                else
                    _db.Database.Migrate();

                _logger.LogInformation("Миграция БД выполнено спустя {0} мс", timer.ElapsedMilliseconds);

                // если в базе уже что-то есть не инициализируем
                if (_db.Books.Any()) { _logger.LogInformation("База данных существует и полна"); return; }

                Random random = new Random();
                _Categories = Enumerable
                    .Range(1, 5)
                    .Select(i => new Category { Name = $"Категория {i}" }).ToArray();

                _Authors = Enumerable
                    .Range(1, 5)
                    .Select(i => new Author
                    {
                        Surname = $"Фамилия {i}",
                        Name = $"Автор {i}",
                        Patronymic = $"Отчество {i}",
                    }).ToArray();

                _Books = Enumerable
                   .Range(1, 500)
                   .Select(i => new Book
                   {
                       Name = $"Книга {i}",
                       Description = $"Описание книги {i}...\nКосмос — опасное место. Тут ты или хищник, или жертва, которую соседи быстро съедят. И если твои зубы недостаточно остры, за твоей спиной обязательно должна стоять сила, способная защитить тебя от других хищников. Раньше такой охраняющей Землю силой была великая раса гэкхо. Но гэкхо пали под натиском врагов, и теперь Земля должна защищать себя сама. И лучше сразу показать всем в космосе, что с человечеством Земли шутки плохи, поскольку у нашей планеты есть не только мощная Армия Земли и собственный космофлот, но и кое-что уникальное, чего нет ни у одной других расы. У Земли есть Комар и его мгновенно перемещающийся по Вселенной древний крейсер, ломающий все привычные рамки и законы войны.",
                       Category = _Categories.ToList().GetRange(random.Next(0, 4), 2),
                       Author = random.NextItem<Author>(_Authors)
                   }).ToArray();
                _Users = Enumerable
                   .Range(1, 5)
                   .Select(i => new User
                   {
                       Surname = $"Фамилия {i}",
                       Name = $"Имя {i}",
                       Patronymic = $"Отчество {i}",
                       Birthday = DateOnly.FromDateTime(DateTime.Now.AddYears(-random.Next(18,51))),
                       Login = $"login{i}",
                       Password = User.HashPassword($"password{i}"),
                       IsAdmin = false,
                       Books = _Books.ToList().GetRange(random.Next(1, 490), 10)
                   }).ToArray();

                _db.AddRange(_Categories);
                _db.AddRange(_Authors);
                _db.AddRange(_Users);
                _db.AddRange(_Books);
                _db.SaveChanges();
                _logger.LogInformation("Инициализация завершена {0} мс", timer.ElapsedMilliseconds);
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Нет доступа к базе данных. Обратитесь в службу поддержки" + Environment.NewLine + ex.Message);
            }
        }
    }
}
