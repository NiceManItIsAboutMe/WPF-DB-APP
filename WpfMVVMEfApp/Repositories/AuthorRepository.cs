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
    internal class AuthorRepository : DbRepository<Author>
    {
        public override IQueryable<Author> Items => base.Items
            .Include(a=>a.Books); 
        public AuthorRepository(BooksDbContext db) : base(db)
        {
        }
    }
}
