using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Repositories.Base;

namespace WpfMVVMEfApp.Repositories
{
    internal static class RepositoriesRegistrator
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services) => services
            .AddTransient<IRepository<Book>, BookRepository>()
            .AddTransient<IRepository<Category>, CategoryRepository>()
            .AddTransient<IRepository<User>, DbRepository<User>>()
            .AddTransient<IRepository<Category>, CategoryRepository>()
            .AddTransient<IRepository<Author>, DbRepository<Author>>()
            .AddTransient<IRepository<BookFile>, DbRepository<BookFile>>()
            ;
    }
}
