using System;
using System.Collections.Generic;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class User: Person
    {
        public string Login { get; set; }

        public byte[] Password { get; set; }

        public DateOnly Birthday { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
