using System.ComponentModel.DataAnnotations;

namespace WpfMVVMEfApp.Models.Base
{
    /// <summary>
    /// Именнованая сущность имеет имя
    /// </summary>
    internal abstract class NamedEntity : Entity
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
