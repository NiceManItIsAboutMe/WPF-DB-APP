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
        // можно вынести команды в отдельный класс, затем только инициализоровать их в конструкторе
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
        #endregion

        public AuthorizationViewModel()
        {
            #region команды
            //создание команды
            CloseApplicationCommand = new RelayCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            #endregion
        }

        private string login = "login";
        public string Login
        {
            get => login;
            set => Set(ref login, value);
        }

        public string Password { get; set; }
    }
}
