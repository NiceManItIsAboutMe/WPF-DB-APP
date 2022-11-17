using Microsoft.Extensions.DependencyInjection;
using WpfMVVMEfApp.ViewModels.MainView;

namespace WpfMVVMEfApp.ViewModels
{
    internal class ViewModelLocator
    {


        public AuthorizationViewModel AuthorizationModel => App.Services.GetRequiredService<AuthorizationViewModel>();

        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();

        public MainViewViewModel MainModel => App.Services.GetRequiredService<MainViewViewModel>();

        public UsersViewModel UsersModel => App.Services.GetRequiredService<UsersViewModel>();
        public AuthorsViewModel AuthorsModel => App.Services.GetRequiredService<AuthorsViewModel>();
        public CategoriesViewModel CategoriesModel => App.Services.GetRequiredService<CategoriesViewModel>();
        public BooksViewModel BooksModel => App.Services.GetRequiredService<BooksViewModel>();
    }
}
