using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class User: Person, ICloneable
    {
        [Required]
        [StringLength(100,MinimumLength =5)]
        public string Login { get; set; }

        public byte[] Password { get; set; }

        public DateTime Birthday { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<Book>? Books { get; set; }

        public static byte[] HashPassword(string Password) => SHA512.HashData(ASCIIEncoding.ASCII.GetBytes(Password));

        public override string ToString()
        {
            return base.ToString() + " " + Login;
        }

        public object Clone()
        {
            ICollection<Book>? books = null;
            if (Books != null)
            {
                books = new ObservableCollection<Book>();
                foreach (var item in Books)
                {
                    books.Add(item.Clone() as Book);
                }
            }
            return new User(Name, Surname, Patronymic, Login, Password, Birthday, IsAdmin, books);
        }

        public User(string name,
            string surname, 
            string patronymic, 
            string login, 
            byte[] password, 
            DateTime birthday, 
            bool isAdmin, 
            ICollection<Book> books)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Login = login;
            Password = password;
            Birthday = birthday;
            IsAdmin = isAdmin;
            Books = books;
        }
        public User(User user):this(user.Name,
            user.Surname,
            user.Patronymic,
            user.Login,
            user.Password,
            user.Birthday,
            user.IsAdmin,
            user.Books) { }

        public User()
        {
        }
    }
}
