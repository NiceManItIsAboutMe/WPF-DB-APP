using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.Models
{
    internal class BookFile : NamedEntity
    {
        public byte[] File { get; set; }

        /// <summary>
        /// Возможно будут храниться файлы в разных форматах
        /// </summary>
        public Book Book { get; set; }
    }
}
