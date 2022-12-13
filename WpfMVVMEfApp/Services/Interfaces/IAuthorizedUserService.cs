using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Services.Interfaces
{
    internal interface IAuthorizedUserService<T> where T : Person
    {
        T User { get; set; }
    }
}
