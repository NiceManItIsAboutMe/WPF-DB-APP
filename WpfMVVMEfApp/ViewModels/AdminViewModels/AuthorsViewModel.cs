using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.Views.AdminViews;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class AuthorsViewModel : ViewModel
    {
        private ApplicationContext _db;

        private IUserDialogService _DialogService;

        private CollectionViewSource _AuthorsViewSource;

        public ICollectionView AuthorsView => _AuthorsViewSource?.View;

        private CollectionViewSource _BooksViewSource;

        public ICollectionView BooksView => _BooksViewSource?.View;


        #region Поиск авторов

        /// <summary> /// Поиск авторов /// </summary>
        private string _AuthorsSearchFilter;

        /// <summary> /// Поиск авторов /// </summary>
        public string AuthorsSearchFilter { get => _AuthorsSearchFilter;
            set
            {
                if (Set(ref _AuthorsSearchFilter, value))
                    _AuthorsViewSource.View.Refresh();
            } }

        #endregion

        #region Поиск книг

        /// <summary> /// Поиск книг /// </summary>
        private string _BooksSearchFilter;

        /// <summary> /// Поиск книг /// </summary>
        public string BooksSearchFilter
        {
            get => _BooksSearchFilter;
            set
            {
                if (Set(ref _BooksSearchFilter, value))
                    _BooksViewSource.View.Refresh();
            }
        }

        #endregion


        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors { get => _Authors; set
            {
                if(Set(ref _Authors, value))
                {
                    _AuthorsViewSource = new CollectionViewSource
                    {
                        Source=Authors,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(Author.Surname),ListSortDirection.Ascending),
                        }
                    };
                    _AuthorsViewSource.Filter += OnAuthorsSearchFilter;
                    OnPropertyChanged(nameof(AuthorsView));
                    _AuthorsViewSource.View.Refresh();
                }

            } }
        #endregion

        #region Выбранный автор

        /// <summary> /// Выбранный автор /// </summary>
        private Author _SelectedAuthor;

        /// <summary> /// Выбранный автор /// </summary>
        public Author SelectedAuthor { get => _SelectedAuthor;
            set
            {
                Set(ref _SelectedAuthor, value);
                if (CanLoadBooksSelectedAuthorsCommandExecute(null))
                    OnLoadBooksSelectedAuthorsCommandExecuted(null);
            } }

        #endregion

        #region Книги

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Book> _Books;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Book> Books
        {
            get => _Books;
            set
            {
                if (Set(ref _Books, value))
                {
                    _BooksViewSource = new CollectionViewSource
                    {
                        Source = Books,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(Book.Name), ListSortDirection.Ascending),
                        }
                    };
                    _BooksViewSource.Filter += OnBooksSearchFilter;
                    OnPropertyChanged(nameof(BooksView));
                    _BooksViewSource.View.Refresh();
                }
            }
        }

        #endregion

        #region Выбранная книга

        /// <summary> /// Выбранная книга /// </summary>
        private Book _SelectedBook;

        /// <summary> /// Выбранная книга /// </summary>
        public Book SelectedBook { get => _SelectedBook; set => Set(ref _SelectedBook, value); }

        #endregion


        #region команда Загрузка авторов

        /// <summary> /// Загрузка авторов /// </summary>
        private ICommand _LoadAuthorsDataCommand;

        /// <summary> /// Загрузка авторов /// </summary>
        public ICommand LoadAuthorsDataCommand => _LoadAuthorsDataCommand
               ??= new RelayCommand(OnLoadAuthorsDataCommandExecuted, CanLoadAuthorsDataCommandExecute);

        /// <summary> /// Загрузка авторов /// </summary>
        public bool CanLoadAuthorsDataCommandExecute(object? p) => true;

        /// <summary> /// Загрузка авторов /// </summary>
        public void OnLoadAuthorsDataCommandExecuted(object? p)
        {
            Authors = new ObservableCollection<Author>(_db.Authors);
        }

        #endregion

        #region команда Загрузка книг выбранной категории

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        private ICommand _LoadBooksSelectedAuthorsCommand;

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public ICommand LoadBooksSelectedCategoriesCommand => _LoadBooksSelectedAuthorsCommand
               ??= new RelayCommand(OnLoadBooksSelectedAuthorsCommandExecuted, CanLoadBooksSelectedAuthorsCommandExecute);

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public bool CanLoadBooksSelectedAuthorsCommandExecute(object? p)
        {
            if (SelectedAuthor == null) return false;
            return true;
        }

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public void OnLoadBooksSelectedAuthorsCommandExecuted(object? p)
        {
            Books = new ObservableCollection<Book>(_db.Books.Where(b => b.Author.Id== SelectedAuthor.Id));
        }

        #endregion



        public AuthorsViewModel(ApplicationContext db, IUserDialogService dialogService)
        {
            _db = db;
            _DialogService = dialogService;
        }

        private void OnBooksSearchFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Book book) || string.IsNullOrEmpty(BooksSearchFilter)) return;

            if (!book.Name.Contains(_BooksSearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }

        private void OnAuthorsSearchFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Author author) || string.IsNullOrEmpty(AuthorsSearchFilter)) return;

            if (!author.Name.Contains(AuthorsSearchFilter, StringComparison.OrdinalIgnoreCase) 
                && !author.Surname.Contains(AuthorsSearchFilter, StringComparison.OrdinalIgnoreCase)
                && !author.Patronymic.Contains(AuthorsSearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }

    }
}
