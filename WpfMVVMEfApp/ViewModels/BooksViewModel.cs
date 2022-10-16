using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Models.PostgreSqlDB;

namespace WpfMVVMEfApp.ViewModels
{
    internal class BooksViewModel:ViewModel
    {
        private ApplicationContext _db;
        public BooksViewModel(ApplicationContext db)
        {
            _db = db;

        }
    }
}
