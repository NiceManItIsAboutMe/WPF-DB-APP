using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using WpfMVVMEfApp.ViewModels.AdminViewModels;

namespace WpfMVVMEfApp.ViewModels
{
    internal static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddScoped<AuthorizationViewModel>()
            .AddScoped<MainWindowViewModel>()
            .AddScoped<AdminViewModel>()
            .AddScoped<UsersViewModel>()
            .AddScoped<BooksViewModel>()
            .AddScoped<AuthorsViewModel>()
            .AddScoped<CategoriesViewModel>()
            ;
    }
}
