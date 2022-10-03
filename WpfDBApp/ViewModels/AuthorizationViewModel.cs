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

        #region поля в авторизации
        private string _Login;
        public string Login
        {
            get => _Login;
            set=>Set(ref _Login, value);
        }
        public string Password { get; set; }
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

        #region Изменение текста в TextBox
        
        public ICommand ChangeTextBoxTextCommand{get;}
        private bool CanChangeTextBoxTextCommandExecute(object p) => true;
        
        private void OnChangeTextBoxTextCommandExecuted(object p)
        {
            if (p.ToString() == LOGIN) { Login = string.Empty; return; }
            if (p.ToString() == PASSWORD) { Password = string.Empty; return; }
            if (p.ToString() == string.Empty) { Login = LOGIN; return; }
            if(p.ToString() == string.Empty) Password = PASSWORD;
        }
        #endregion
        #endregion

        public AuthorizationViewModel()
        {
            #region поля
            Login = LOGIN;
            Password = PASSWORD;
            #endregion
            #region команды
            //создание команды
            CloseApplicationCommand = new RelayCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            ChangeTextBoxTextCommand = new RelayCommand(OnChangeTextBoxTextCommandExecuted, CanChangeTextBoxTextCommandExecute);
            #endregion
        }
    }
}
