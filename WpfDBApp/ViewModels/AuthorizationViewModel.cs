using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDBApp.Base.ViewModels;

namespace WpfDBApp.ViewModels
{
    internal class AuthorizationViewModel : ViewModel
    {
        private string login = "login";

        public string Login
        {
            get => login;
            set => Set(ref login, value);
        }

        public string Password { get; set; }
    }
}
