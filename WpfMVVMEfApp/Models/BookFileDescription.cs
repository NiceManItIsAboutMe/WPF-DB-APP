using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class BookFileDescription : NamedEntity
    {
        public BookFile File { get; set; }

        /// <summary>
        /// Возможно будут храниться файлы в разных форматах
        /// </summary>
        public Book Book { get; set; }
    }
}
