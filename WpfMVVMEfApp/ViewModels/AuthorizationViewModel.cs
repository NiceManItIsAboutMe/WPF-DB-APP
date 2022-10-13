using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
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
        private ApplicationContext _db;

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

        #region Команда входа
        public ICommand SignInCommand { get; set; }

        public bool CanSignInCommandExecute(object? p) => true;

        public async void OnSignInCommandExecuted(object? p)
        {
            if (String.IsNullOrEmpty(Login)) { MessageBox.Show("Введите логин"); return; }
            if (String.IsNullOrEmpty(Password)) { MessageBox.Show("Введите пароль"); return; }
            try
            {
                var password = User.HashPassword(Password);
                
                var user =await _db.Users.Where(u => u.Login == Login && u.Password == password).FirstOrDefaultAsync();
                
                if (user==null) { MessageBox.Show("Вы ввели неверный логин или пароль"); return; }
                else
                {
                    MainWindow window = new MainWindow();
                    window.Show();
                    App.Current.Windows[0].Close();
                }
            }
            catch(NpgsqlException ex)
            {
                MessageBox.Show("Нет доступа к базе данных. Обратитесь в службу поддержки" + Environment.NewLine
                    +"Код ошибки:" + ex.ErrorCode
                    + Environment.NewLine + ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                MessageBox.Show("Нет доступа к базе данных. Обратитесь в службу поддержки" + Environment.NewLine + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка. Обратитесь в службу поддержки." + Environment.NewLine + ex.Message);
            }
        }
        #endregion

        public AuthorizationViewModel(ApplicationContext db)
        {
            _db = db;
            SignInCommand = new RelayCommand(OnSignInCommandExecuted, CanSignInCommandExecute);

        }
    }
}
