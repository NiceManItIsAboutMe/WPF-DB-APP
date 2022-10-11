using System.Collections.Generic;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class Author : Person
    {
        public virtual ICollection<Book> Books { get; set; }
    }
}
