using System.ComponentModel.DataAnnotations;

namespace WpfMVVMEfApp.Models.Base
{
    /// <summary>
    /// Имеет фамили и имя
    /// </summary>
    internal abstract class Person : NamedEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public virtual string Surname { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public virtual string Patronymic { get; set; }

        public override string ToString()
        {
            return Surname + " " + Name + " " + Patronymic;
        }
    }
}
