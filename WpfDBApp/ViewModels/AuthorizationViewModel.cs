using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfDBApp.Base.ViewModels;
using WpfDBApp.ViewModels.Commands;

namespace WpfDBApp.ViewModels
{
    internal class AuthorizationViewModel : ViewModel
    {
        #region CONST
        public const string LOGIN = "Введите логин";
        public const string PASSWORD = "Введите пароль";
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
        public ICommand SignInCommand { get; }

        private bool CanSignInCommandExecute(object p) => true;
        private void OnSignInCommandExecuted(object p)
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
            if(Login=="1" && Password=="1")
                MessageBox.Show("вошли");

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
