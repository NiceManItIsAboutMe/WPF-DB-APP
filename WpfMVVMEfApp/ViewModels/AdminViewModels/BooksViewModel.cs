using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class BooksViewModel :ViewModel
    {
        private ApplicationContext _db;

        private IUserDialogService _DialogService;

        public BooksViewModel(ApplicationContext db, IUserDialogService dialogService)
        {
            _db = db;
            _DialogService = dialogService;
        }
    }
}
