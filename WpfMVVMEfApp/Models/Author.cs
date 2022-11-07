using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class Author : Person, ICloneable
    {
        public virtual ICollection<Book> Books { get; set; }

        public Author(string name,string surname,string patronymic,ICollection<Book> books)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Books = books;
        }
        public Author(Author author):this(author.Name,author.Surname,author.Patronymic,author.Books)
        {

        }
        public Author()
        {

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

            return new Author(Name, Surname, Patronymic, books);
        }
    }
}
