using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class Book : NamedEntity
    {
        public string Description { get; set; }

        //virtual - ленивая загрузка (EF будет забирать из базы только в момент обращения) (грабля в двух концах )0)0
        public virtual Category Category { get; set; }

        public virtual Author Author { get; set; }
    }
}
