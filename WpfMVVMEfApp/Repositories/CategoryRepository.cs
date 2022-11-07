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
    internal class CategoryRepository : DbRepository<Category>
    {
        public override IQueryable<Category> Items => base.Items
            .Include(c=>c.Books);
        public CategoryRepository(BooksDbContext db) : base(db)
        {
        }
    }
}
