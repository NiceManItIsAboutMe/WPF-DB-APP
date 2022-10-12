using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xaml;
using WpfDBApp.ViewModels.Base;

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


        public AuthorizationViewModel()
        {
            
        }
    }
}
