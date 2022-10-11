namespace WpfMVVMEfApp.Models.Base
{
    /// <summary>
    /// Имеет фамили и имя
    /// </summary>
    internal abstract class Person : NamedEntity
    {
        public string Surname { get; set; }

        public string Patronymic { get; set; }
    }
}
