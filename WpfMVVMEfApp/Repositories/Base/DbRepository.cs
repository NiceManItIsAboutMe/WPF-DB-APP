using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfMVVMEfApp.Models.Base;
using WpfMVVMEfApp.Models.PostgreSqlDB;

namespace WpfMVVMEfApp.Repositories.Base
{
    internal class DbRepository<T> : IRepository<T> where T : Entity, new()
    {
        private readonly BooksDbContext _db;
        private readonly DbSet<T> _Set;

        public bool IsAutoSaveChanges { get; set; } = true;
        public IQueryable<T> Items => _Set;

        public DbRepository(BooksDbContext db)
        {
            _db = db;
            _Set = _db.Set<T>();
        }

        public T Add(T item)
        {
            if (item == null) throw new NullReferenceException(nameof(item));

            _Set.Add(item);
            if (IsAutoSaveChanges)
                _db.SaveChanges();
            return item;
        }

        public async Task<T> AddAsync(T item, CancellationToken Cancel = default)
        {
            if (item == null) throw new NullReferenceException(nameof(item));

            _Set.Add(item);
            if (IsAutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
            return item;
        }

        // если не 1 запись возвращается будет кинута ошибка,
        // Id должен быть уникальным ключом и будет понятно, что с бд че то не так
        public T Get(int id) => Items.SingleOrDefault(item => item.Id == id);

        public async Task<T> GetAsync(int id, CancellationToken Cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.Id == id, Cancel)
            .ConfigureAwait(false);

        public void Remove(int id)
        {
            _db.Remove(new T { Id = id });
            if (IsAutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task RemoveAsync(int id, CancellationToken Cancel = default)
        {
            _db.Remove(new T { Id = id });
            if (IsAutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        public void Update(T item)
        {
            if (item == null) throw new NullReferenceException(nameof(item));

            _Set.Update(item);
            if (IsAutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task UpdateAsync(T item, CancellationToken Cancel = default)
        {
            if (item == null) throw new NullReferenceException(nameof(item));

            _Set.Update(item);
            if (IsAutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }
    }
}
