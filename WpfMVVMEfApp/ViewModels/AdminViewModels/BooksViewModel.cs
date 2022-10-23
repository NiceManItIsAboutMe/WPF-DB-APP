using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class BooksViewModel :ViewModel
    {
        private ApplicationContext _db;

        private IUserDialogService _DialogService;

        #region Книги

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Book> _Books;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Book> Books { get => _Books; set => Set(ref _Books, value); }

        #endregion



        public BooksViewModel(ApplicationContext db, IUserDialogService dialogService)
        {
            _db = db;
            _DialogService = dialogService;
            Books = new ObservableCollection<Book>(db.Books);
        }
    }
}
