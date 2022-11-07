using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class Category : NamedEntity, ICloneable
    {
        //virtual - ленивая загрузка (EF будет забирать из базы только в момент обращения) (грабля в двух концах )0)0
        public virtual ICollection<Book>? Books { get; set; }

        public Category(string name,ICollection<Book>? books)
        {
            Name = name;
            Books = books;
        }
        public Category(Category category): this(category.Name, category.Books)
        {
        }

        public Category()
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
            return new Category(Name, books);
        }
    }
}
