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

namespace WpfDBApp.ViewModels
{
    internal class AuthorizationViewModel : ViewModel
    {
        #region CONST
        public const string LOGIN = "Введите логин";
        public const string PASSWORD = "Введите пароль";
        private static readonly string _connectionString = "Host=localhost:5432; Username=postgres; Password=postgres; Database=WpfDBApp";
        #endregion

        #region поля в окне
        private string _Login;
        public string Login
        {
            get => _Login;
            set=>Set(ref _Login, value);
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
        /*await using var cmd1 = new NpgsqlCommand("INSERT INTO public.users(name, surname, patronymic, description, superuser, login, password) VALUES('name','surname','patronymic','description', FALSE,$1,$2)", connection)
{ 
    Parameters =
    {
        new(){Value=Login},
        new(){Value=hash16}
    }
};
await cmd1.ExecuteNonQueryAsync();*/
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
            var hash16 = ByteArrayToString16(hash);
            //соединение
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                //смотрим есть ли в базе такой человек
                await using var cmd = new NpgsqlCommand("SELECT login,password FROM public.users WHERE login=$1 AND password=$2", connection)
                {
                    Parameters =
                {
                    new(){Value=Login},
                    new(){Value=hash16}
                }
                };
                await using var reader = cmd.ExecuteReader();

                //если есть заходим
                if (reader.HasRows)
                    MessageBox.Show("вошли");
                else
                    MessageBox.Show("Вы ввели неверный логин или пароль");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Нет доступа к базе данных, пожалуйста обратитесь к системному администратору и сообщите об ошибке.\n Ошибка:"+ex.Message);
            }
        }

        /// <summary>
        /// преобразует массив байтов в 16 формат (с офиц сайта microsoft https://learn.microsoft.com/ru-ru/troubleshoot/developer/visualstudio/csharp/language-compilers/compute-hash-values)
        /// </summary>
        /// <param name="arrInput">массив байтов</param>
        /// <returns>строка в 16ричной форме</returns>
        private static string ByteArrayToString16(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
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
