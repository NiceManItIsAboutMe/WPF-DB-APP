using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xaml;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Views;

namespace WpfMVVMEfApp.ViewModels
{
    internal class AuthorizationViewModel:ViewModel
    {
        #region поля
        #region Заголовок string Title

        /// <summary> /// Заголовок /// </summary>
        private string _Title;

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

        #region Команда входа
        public ICommand SignInCommand { get; set; }

        public bool CanSignInCommandExecute(object? p) => true;

        public async void OnSignInCommandExecuted(object? p)
        {
            if (String.IsNullOrEmpty(Login)) { MessageBox.Show("Введите логин"); return; }
            if (String.IsNullOrEmpty(Password)) { MessageBox.Show("Введите пароль"); return; }

            var password = User.HashPassword(Password);
            using var db = App.Services.GetRequiredService<ApplicationContext>();

            var user = await db.Users.Where(u => u.Login == Login && u.Password == password).FirstAsync();

            if (user == null) { MessageBox.Show("Вы ввели неверный логин или пароль"); return; }
            else
            {
                MainWindow window = new MainWindow();
                window.Show();
                App.Current.Windows[0].Close();
            }
        }


        #endregion
        public AuthorizationViewModel()
        {
            SignInCommand = new RelayCommand(OnSignInCommandExecuted, CanSignInCommandExecute);


        }
    }
}
