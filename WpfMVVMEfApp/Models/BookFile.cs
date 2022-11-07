using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class BookFile : NamedEntity, ICloneable
    {
        public byte[] File { get; set; }

        /// <summary>
        /// Возможно будут храниться файлы в разных форматах
        /// </summary>
        public Book Book { get; set; }
        public BookFile(string name,byte[] file,Book book)
        {
            Name = name;
            File = file;
            Book = Book;
        }
        public BookFile(BookFile bookFile):this(bookFile.Name,bookFile.File,bookFile.Book)
        {

        }
        public BookFile()
        {

        }

        public object Clone()
        {
            Book? book = null;
            if (Book != null)
                book = Book.Clone() as Book;
            return new BookFile(Name, File, book);
        }
    }
}
