using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class User: Person
    {
        [Required]
        [StringLength(100,MinimumLength =5)]
        public string Login { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public byte[] Password { get; set; }

        public DateOnly Birthday { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public static byte[] HashPassword(string Password) => SHA512.HashData(ASCIIEncoding.ASCII.GetBytes(Password));

        public override string ToString()
        {
            return base.ToString() + " " + Login;
        }
    }
}
