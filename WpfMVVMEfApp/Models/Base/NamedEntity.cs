namespace WpfMVVMEfApp.Models.Base
{
    /// <summary>
    /// Именнованая сущность имеет имя
    /// </summary>
    internal abstract class NamedEntity : Entity
    {
        public virtual string Name { get; set; }
    }
}
