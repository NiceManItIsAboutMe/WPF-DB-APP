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
using WpfMVVMEfApp.ViewModels.AdminViewModels.Editors;
using static WpfMVVMEfApp.ViewModels.AdminViewModels.Editors.BookEditorViewModel;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class BooksViewModel : ViewModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbFactory;

        private IUserDialogService _DialogService;

        #region Строка поиска

        /// <summary> /// Строка поиска /// </summary>
        private string _SearchFilter;

        /// <summary> /// Строка поиска /// </summary>
        public string SearchFilter
        {
            get => _SearchFilter;
            set
            {
                if (Set(ref _SearchFilter, value))
                    _BooksViewSource.View.Refresh();
            }
        }

        #endregion

        private CollectionViewSource _BooksViewSource;

        /// <summary>/// Отображение пользователей /// </summary>
        public ICollectionView BooksView => _BooksViewSource?.View;

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
                    _BooksViewSource.Filter += OnBooksViewSourceFilter;
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

        #region команда Загрузки книг

        /// <summary> /// Загрузки книг /// </summary>
        private ICommand _LoadBooksCommand;

        /// <summary> /// Загрузки книг /// </summary>
        public ICommand LoadBooksCommand => _LoadBooksCommand
               ??= new RelayCommand(OnLoadBooksCommandExecuted, CanLoadBooksCommandExecute);

        /// <summary> /// Загрузки книг /// </summary>
        public bool CanLoadBooksCommandExecute(object? p) => true;

        /// <summary> /// Загрузки книг /// </summary>
        public async void OnLoadBooksCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Books = new ObservableCollection<Book>(await db.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .Include(b=>b.BookFilesDescription)
                .OrderBy(b => b.Name)
                .AsNoTracking()
                .ToListAsync());
        }

        #endregion

        #region команда Удаление книги

        /// <summary> /// Удаление книги /// </summary>
        private ICommand _RemoveSelectedBookCommand;

        /// <summary> /// Удаление книги /// </summary>
        public ICommand RemoveSelectedBookCommand => _RemoveSelectedBookCommand
               ??= new RelayCommand(OnRemoveSelectedBookCommandExecuted, CanRemoveSelectedBookCommandExecute);

        /// <summary> /// Удаление книги /// </summary>
        public bool CanRemoveSelectedBookCommandExecute(object? p) => true;

        /// <summary> /// Удаление книги /// </summary>
        public async void OnRemoveSelectedBookCommandExecuted(object? p)
        {
            if (SelectedBook == null) return;

            if (!_DialogService.Confirm($"Вы действительно хотите удалить книгу: {SelectedBook.Name}" +
                $", автора: {SelectedBook.Author}?",
                "Удаление"))
                return;

            using var db = await _dbFactory.CreateDbContextAsync();
            db.Remove(SelectedBook);
            await db.SaveChangesAsync();

            Books.Remove(SelectedBook);
        }

        #endregion

        #region команда Редактирование книги

        /// <summary> /// Редактирование книги /// </summary>
        private ICommand _EditSelectedBookCommand;

        /// <summary> /// Редактирование книги /// </summary>
        public ICommand EditSelectedBookCommand => _EditSelectedBookCommand
               ??= new RelayCommand(OnEditSelectedBookCommandExecuted, CanEditSelectedBookCommandExecute);

        /// <summary> /// Редактирование книги /// </summary>
        public bool CanEditSelectedBookCommandExecute(object? p) => SelectedBook is Book;

        /// <summary> /// Редактирование книги /// </summary>
        public async void OnEditSelectedBookCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();

            Book book = await db.Books.Include(b => b.Categories).Include(b => b.Author).Include(b=>b.BookFilesDescription)
                .FirstAsync(b => b.Id == SelectedBook.Id);
            BookEditorViewModel vm = new BookEditorViewModel(
                book,
                _DialogService,
                new ObservableCollection<Category>(await db.Categories.ToListAsync()),
                new ObservableCollection<Author>(await db.Authors.ToListAsync()));

            bool result = _DialogService.Edit(vm);
            if (!result) return;

            await db.SaveChangesAsync();
            Books.Remove(SelectedBook);
            SelectedBook = book;
            Books.Add(SelectedBook);

            _BooksViewSource.View.Refresh();
        }

        #endregion

        #region команда Добавить книгу

        /// <summary> /// Добавить книгу /// </summary>
        private ICommand _AddBookCommand;

        /// <summary> /// Добавить книгу /// </summary>
        public ICommand AddBookCommand => _AddBookCommand
               ??= new RelayCommand(OnAddBookCommandExecuted, CanAddBookCommandExecute);

        /// <summary> /// Добавить книгу /// </summary>
        public bool CanAddBookCommandExecute(object? p) => true;

        /// <summary> /// Добавить книгу /// </summary>
        public async void OnAddBookCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Book book = new Book();
            BookEditorViewModel vm = new BookEditorViewModel(
                book,
                _DialogService,
                new ObservableCollection<Category>(await db.Categories.ToListAsync()),
                new ObservableCollection<Author>(await db.Authors.ToListAsync()));

            bool result = _DialogService.Edit(vm);
            if (!result) return;

            db.Add(book);
            await db.SaveChangesAsync();
            SelectedBook = book;
            Books.Add(SelectedBook);
        }

        #endregion


        public BooksViewModel(IDbContextFactory<ApplicationDbContext> dbFactory, IUserDialogService dialogService)
        {
            _dbFactory = dbFactory;
            _DialogService = dialogService;
        }

        private void OnBooksViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Book book) || string.IsNullOrEmpty(SearchFilter)) return;

            if (!book.Name.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }

    }
}
