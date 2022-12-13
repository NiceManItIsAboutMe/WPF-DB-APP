using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.Services
{
    internal class AuthorizedUserService : ViewModel, IAuthorizedUserService<User>
    {
        private static User _User;

        public User User { get => _User; set => _User = value; }
    }
}
