namespace WpfMVVMEfApp.Models.Base
{
    /// <summary>
    /// Имеет фамили и имя
    /// </summary>
    internal abstract class Person : NamedEntity
    {
        public virtual string Surname { get; set; }

        public virtual string Patronymic { get; set; }

        public override string ToString()
        {
            return Surname + " " + Name + " " + Patronymic;
        }
    }
}
