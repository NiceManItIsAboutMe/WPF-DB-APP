using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Repositories.Base;

namespace WpfMVVMEfApp.Repositories
{
    internal class BookRepository : DbRepository<Book>
    {
        public override IQueryable<Book> Items => base.Items
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .Include(b => b.BookFiles)
            .Include(b => b.Users)
            ;
        public BookRepository(BooksDbContext db) : base(db)
        {
        }
    }
}
