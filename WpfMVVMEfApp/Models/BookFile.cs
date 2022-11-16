using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class BookFile : Entity
    {
        public byte[] File { get; set; }

        public int BookFileDescriptionId { get; set; }
        public BookFileDescription BookFileDescription { get; set; }
    }
}
