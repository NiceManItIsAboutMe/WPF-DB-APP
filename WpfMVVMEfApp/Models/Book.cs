using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using WpfMVVMEfApp.Models.Base;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMVVMEfApp.Models
{
    // можно будет добавить поле цикл книг (серия книг)
    internal class Book : NamedEntity, ICloneable
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

        public Book(string name,
            string description, 
            ICollection<Category>? categories, 
            Author author, 
            ICollection<BookFile>? bookFiles, 
            ICollection<User>? users)
        {
            Name = name;
            Description = description;
            Categories = categories;
            Author = author;
            BookFiles = bookFiles;
            Users = users;
        }

        public Book(Book book):this(book.Name,book.Description,book.Categories,book.Author,book.BookFiles,book.Users)
        {

        }

        public Book()
        {

        }

        public object Clone()
        {
            ICollection<Category>? categories = null;
            ICollection<BookFile>? bookFiles = null;
            ICollection<User>? users = null;
            Author author = null;

            if (Categories != null)
            {
                categories = new ObservableCollection<Category>();
                foreach (var item in Categories)
                {
                    categories.Add(item.Clone() as Category);
                }
            }
            if (BookFiles != null)
            {
                bookFiles = new ObservableCollection<BookFile>();
                foreach (var item in BookFiles)
                {
                    bookFiles.Add(item.Clone() as BookFile);
                }
            }
            if (Users != null)
            {
                users = new ObservableCollection<User>();
                foreach (var item in Users)
                {
                    users.Add(item.Clone() as User);
                }
            }
            if (Author != null)
                author = Author.Clone() as Author;

            return new Book(Name,Description,categories,author,bookFiles,users);
        }
    }
}
