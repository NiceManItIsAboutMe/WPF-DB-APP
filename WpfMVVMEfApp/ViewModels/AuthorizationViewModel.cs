using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDBApp.ViewModels.Base;

namespace WpfMVVMEfApp.ViewModels
{
    internal class AuthorizationViewModel:ViewModel
    {
        #region Заголовок
        private string _Title = "Авторизация";

        public string Title { get => _Title; set => Set(ref _Title, value); }
        #endregion
    }
}
