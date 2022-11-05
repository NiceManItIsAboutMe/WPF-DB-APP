using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    // можно будет добавить поле цикл книг (серия книг)
    internal class Book : NamedEntity
    {
        [Required(ErrorMessage = "Не указано описание")]
        [StringLength(10000, MinimumLength = 50)]
        public string Description { get; set; }

        //virtual - ленивая загрузка (EF будет забирать из базы только в момент обращения) (грабля в двух концах )0)0
        public virtual ICollection<Category>? Categories { get; set; }

        [Required]
        public virtual Author Author { get; set; }

        public virtual ICollection<BookFile>? BookFiles { get; set; }

        public virtual ICollection<User>? Users { get; set; }

    }
}
