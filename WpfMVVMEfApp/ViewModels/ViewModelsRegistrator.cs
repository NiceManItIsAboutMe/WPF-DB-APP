using Microsoft.Extensions.DependencyInjection;
using WpfMVVMEfApp.ViewModels.MainView;

namespace WpfMVVMEfApp.ViewModels
{
    internal static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddScoped<AuthorizationViewModel>()
            .AddScoped<MainWindowViewModel>()
            .AddScoped<MainViewViewModel>()
            .AddScoped<UsersViewModel>()
            .AddScoped<BooksViewModel>()
            .AddScoped<AuthorsViewModel>()
            .AddScoped<CategoriesViewModel>()
            ;
    }
}
