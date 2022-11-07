
using WpfMVVMEfApp.Repositories.Base;

namespace WpfMVVMEfApp.Models.Base
{
    /// <summary>
    /// Сущность имеет id
    /// </summary>
    internal abstract class Entity :IEntity
    {
        public int Id { get; set; }
    }
}
