using Microsoft.Extensions.DependencyInjection;
using WpfMVVMEfApp.ViewModels.AdminViewModels;

namespace WpfMVVMEfApp.ViewModels
{
    //Задать в App.xaml
    /*<Application.Resources>
        <ResourceDictionary>
            <vm:ViewModelLocator x:Key="Locator"/>
        </ResourceDictionary>
    </Application.Resources>*/

    //вместо локатора можно создать некий контроллер который в конструктор будет принимать все сервисы и хранить их в себе
    //тоже самое но вместо обращения к App сервисы будут вытакскиваться из конструктора
    internal class ViewModelLocator
    {


        public AuthorizationViewModel AuthorizationModel => App.Services.GetRequiredService<AuthorizationViewModel>();

        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();

        #region AdminViewModel
        public AdminViewModel AdminModel => App.Services.GetRequiredService<AdminViewModel>();

        public UsersViewModel UsersModel => App.Services.GetRequiredService<UsersViewModel>();
        public AuthorsViewModel AuthorsModel => App.Services.GetRequiredService<AuthorsViewModel>();
        public CategoriesViewModel CategoriesModel => App.Services.GetRequiredService<CategoriesViewModel>();
        public BooksViewModel BooksModel => App.Services.GetRequiredService<BooksViewModel>();
        #endregion

        #region NotAdminViewModel

        #endregion

    }
}
