using System;
using System.Windows;
using System.Windows.Input;
using WpfDBApp.Base.ViewModels;
using WpfDBApp.ViewModels.Commands;
using System.Security.Cryptography;
using Npgsql;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using WpfDBApp.Models;
using WpfDBApp.Views;
using WpfDBApp.Database;
using System.Security.Policy;

namespace WpfDBApp.ViewModels
{
    internal class AuthorizationViewModel : ViewModel
    {

        #region CONST
        public static readonly string LOGIN = "Введите логин";
        public static readonly string PASSWORD = "Введите пароль";
        #endregion

        #region поля в окне
        private string _Login;
        public string Login
        {
            get => _Login;
            set => Set(ref _Login, value);
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set => Set(ref _Password, value);
        }
        #endregion

        // можно вынести команды в отдельный класс и вызывать их сразу из view xaml
        #region Команды
        #region Закрытие (тест комманд)
        /// <summary>
        /// объявление команды
        /// </summary>
        public ICommand CloseApplicationCommand { get; }

        /// <summary>
        /// условие при котором можно выполнить команду (тут в любом случае выполняем)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CanCloseApplicationCommandExecute(object p) => true;
        /// <summary>
        /// выполнение самой команды
        /// </summary>
        /// <param name="p"></param>
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion
        #region кнопка входа
        public ICommand SignInCommand { get; }

        private bool CanSignInCommandExecute(object p) => true;
        private async void OnSignInCommandExecuted(object p)
        {
            //новое окно смотря кто зашел
            if (Login.Length == 0)
            {
                MessageBox.Show("Введите логин");
                return;
            }
            if (Password.Length == 0)
            {
                MessageBox.Show("Введите пароль");
                return;
            }
            //хэшим пароль, чтобы сравнить с базой
            var hash = SHA512.HashData(ASCIIEncoding.ASCII.GetBytes(Password));
            //соединение

            try
            {
                await using var db = new Connection();
                await using var connection = db.GetConnection();
                /*await using var cmd1 = new NpgsqlCommand("INSERT INTO public.users(name, surname, patronymic, description, login, password) VALUES('name','surname','patronymic','description',$1,$2)", connection)
                {
                    Parameters =
    {
        new(){Value=Login},
        new(){Value=hash}
    }
                };
                await cmd1.ExecuteNonQueryAsync();*/
                //смотрим есть ли в базе такой человек
                await using var cmd = new NpgsqlCommand("SELECT public.positions.issuperuser FROM positions,users WHERE users.login=$1 AND users.password=$2 AND users.positionid=positions.id", connection)
                //await using var cmd = new NpgsqlCommand("SELECT password FROM users WHERE users.login=$1 AND users.password=$2", connection)
                {
                    Parameters =
                    {
                    new(){Value=Login},
                    new(){Value=hash}
                    }
                };
                await using var reader = cmd.ExecuteReader();
                //если есть заходим
                if (reader.HasRows)
                {
                    //создаем основное окно
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    //закрываем окно авторизации
                    Application.Current.Windows[0].Close();
                }
                else
                    MessageBox.Show("Вы ввели неверный логин или пароль");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Нет доступа к базе данных, пожалуйста обратитесь к системному администратору и сообщите об ошибке.\n Ошибка:" + ex.Message);
            }
        }


        #endregion
        #endregion

        public AuthorizationViewModel()
        {
            #region поля
            Login = String.Empty;
            Password = String.Empty;
            #endregion
            #region команды
            //создание команды
            CloseApplicationCommand = new RelayCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            SignInCommand = new RelayCommand(OnSignInCommandExecuted, CanSignInCommandExecute);
            #endregion

        }
    }
}
