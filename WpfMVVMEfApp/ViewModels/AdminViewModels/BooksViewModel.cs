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
using static WpfMVVMEfApp.ViewModels.Editors.BookEditorViewModel;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class BooksViewModel : ViewModel
    {
        private ApplicationContext _db;

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
        public void OnLoadBooksCommandExecuted(object? p)
        {
            Books = new ObservableCollection<Book>(_db.Books.Include(b=> b.Author).Include(b=>b.Categories).OrderBy(b=> b.Name).AsNoTracking());
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

            Book book = await _db.Books.FirstAsync(b => b.Id == SelectedBook.Id);
            if (!_DialogService.Confirm($"Вы действительно хотите удалить книгу: {SelectedBook.Name}" +
                $", автора: {SelectedBook.Author}?",
                "Удаление"))
                return;

            _db.Remove(book);
            await _db.SaveChangesAsync();

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
            Book book = await _db.Books.Include(b => b.Categories).Include(b => b.Author).FirstAsync(b => b.Id == SelectedBook.Id);
            bool result=_DialogService.Edit(book);
            if (!result)
            {
                // перестаем остлеживать данную сущность, иначе при следующем входе в редактор мы получим изменненую сущность, которая была закэширована EF
                _db.Entry(book).State = EntityState.Detached;
                return;
            }
            try
            {
                _db.Update(book);
                await _db.SaveChangesAsync();
                _db.Entry(book).State = EntityState.Detached;
                Books.Remove(SelectedBook);
                Books.Add(book);
                SelectedBook = book;
                _BooksViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }

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
            Book book = new Book();
            bool result = _DialogService.Edit(book);
            if (!result) return;
            try
            {
                _db.Add(book);
                await _db.SaveChangesAsync();
                Books.Add(book);
                SelectedBook = book;
                _BooksViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                _db.Remove(book);
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
        }

        #endregion


        public BooksViewModel(ApplicationContext db, IUserDialogService dialogService)
        {
            _db = db;
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
