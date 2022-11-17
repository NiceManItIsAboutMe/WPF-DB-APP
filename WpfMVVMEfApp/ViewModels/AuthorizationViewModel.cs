using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels.AdminViewModels;

namespace WpfMVVMEfApp.ViewModels
{
    internal class AuthorizationViewModel : ViewModel
    {
        #region поля
        private IUserDialogService _DialogService;
        private IDbContextFactory<ApplicationDbContext> _dbFactory;
        public MainWindowViewModel _MainWindowViewModel { get; set; }
        private AdminViewModel _AdminViewModel;

        #region Заголовок string Title

        /// <summary> /// Заголовок /// </summary>
        private string _Title = "Авторизация";

        /// <summary> /// Заголовок /// </summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion


        #region Пароль string Password

        /// <summary> /// Пароль /// </summary>
        private string _Password;

        /// <summary> /// Пароль /// </summary>
        public string Password { get => _Password; set => Set(ref _Password, value); }

        #endregion


        #region Логин string Login

        /// <summary> /// Логин /// </summary>
        private string _Login;

        /// <summary> /// Логин /// </summary>
        public string Login { get => _Login; set => Set(ref _Login, value); }

        #endregion


        #endregion

        #region команда Команда входа

        /// <summary> /// Команда входа /// </summary>
        private ICommand _SignInCommand;

        /// <summary> /// Команда входа /// </summary>
        public ICommand SignInCommand => _SignInCommand
               ??= new RelayCommand(OnSignInCommandExecuted, CanSignInCommandExecute);

        /// <summary> /// Команда входа /// </summary>
        public bool CanSignInCommandExecute(object? p) => true;

        /// <summary> /// Команда входа /// </summary>
        public async void OnSignInCommandExecuted(object? p)
        {
            if (String.IsNullOrEmpty(Login)) { _DialogService.ShowWarning("Введите логин", "Предупреждение"); return; }
            if (String.IsNullOrEmpty(Password)) { _DialogService.ShowWarning("Введите пароль", "Предупреждение"); return; }

            var password = User.HashPassword(Password);
            using (var db = await _dbFactory.CreateDbContextAsync())
            {
                var user = await db.Users
                    .Where(u => u.Login == Login && u.Password == password)
                    .Include(u => u.Books)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (user == null) { _DialogService.ShowWarning("Вы ввели неверный логин или пароль", "Предупреждение"); return; }
                else
                {
                    _MainWindowViewModel.User = user;
                    _MainWindowViewModel.CurrrentViewModel = _AdminViewModel;
                    Login = String.Empty;
                    Password = String.Empty;
                }
            }
        }
        #endregion

        public AuthorizationViewModel(IDbContextFactory<ApplicationDbContext> dbFactory, 
            AdminViewModel adminViewModel, 
            IUserDialogService dialogService)
        {
            _dbFactory = dbFactory;
            _DialogService = dialogService;
            _AdminViewModel = adminViewModel;
        }
    }
}
