using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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

        public byte[] HashPassword(string Password) => SHA512.HashData(ASCIIEncoding.ASCII.GetBytes(Password));
    }
}
