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
using static WpfMVVMEfApp.ViewModels.Editors.BookEditorViewModel;

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
            Authors = new ObservableCollection<Author>(_db.Authors.AsNoTracking());
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
            Books = new ObservableCollection<Book>(_db.Books.Where(b => b.Author.Id== SelectedAuthor.Id)
                .Include(b => b.Categories).Include(b => b.Author).AsNoTracking());
        }

        #endregion



        #region команда Удаление автора

        /// <summary> /// Удаление автора /// </summary>
        private ICommand _RemoveSelectedAuthorCommand;

        /// <summary> /// Удаление автора /// </summary>
        public ICommand RemoveSelectedAuthorCommand => _RemoveSelectedAuthorCommand
               ??= new RelayCommand(OnRemoveSelectedAuthorCommandExecuted, CanRemoveSelectedAuthorCommandExecute);

        /// <summary> /// Удаление автора /// </summary>
        public bool CanRemoveSelectedAuthorCommandExecute(object? p) => true;

        /// <summary> /// Удаление автора /// </summary>
        public async void OnRemoveSelectedAuthorCommandExecuted(object? p)
        {
            if (SelectedAuthor == null) return;
            Author author = await _db.Authors.FirstAsync(a => a.Id == SelectedAuthor.Id);
            if (!_DialogService.Confirm($"Вы действительно хотите удалить автора: {SelectedAuthor}\n При удалении автора удалятся так же и все его книги!!!", "Удаление"))
                return;
            _db.Remove(author);
            await _db.SaveChangesAsync();
            Authors.Remove(SelectedAuthor);
        }

        #endregion

        #region команда Редактирование автора

        /// <summary> /// Редактирование автора /// </summary>
        private ICommand _EditAuthorCommand;

        /// <summary> /// Редактирование автора /// </summary>
        public ICommand EditAuthorCommand => _EditAuthorCommand
               ??= new RelayCommand(OnEditAuthorCommandExecuted, CanEditAuthorCommandExecute);

        /// <summary> /// Редактирование автора /// </summary>
        public bool CanEditAuthorCommandExecute(object? p) => SelectedAuthor is Author;

        /// <summary> /// Редактирование автора /// </summary>
        public async void OnEditAuthorCommandExecuted(object? p)
        {
            Author author = await _db.Authors.FirstAsync(a=>a.Id==SelectedAuthor.Id);
            bool result = _DialogService.Edit(author);
            if (!result)
            {
                // перестаем остлеживать данную сущность, иначе при следующем входе в редактор мы получим изменненую сущность, которая была закэширована EF
                _db.Entry(author).State = EntityState.Detached;
                return;
            }
            try
            {
                _db.Update(author);
                await _db.SaveChangesAsync();
                _db.Entry(author).State = EntityState.Detached;
                Authors.Remove(SelectedAuthor);
                Authors.Add(author);
                SelectedAuthor = author;
                _AuthorsViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
        }

        #endregion

        #region команда Добавить автора

        /// <summary> /// Добавить автора /// </summary>
        private ICommand _AddAuthorCommand;

        /// <summary> /// Добавить автора /// </summary>
        public ICommand AddAuthorCommand => _AddAuthorCommand
               ??= new RelayCommand(OnAddAuthorCommandExecuted, CanAddAuthorCommandExecute);

        /// <summary> /// Добавить автора /// </summary>
        public bool CanAddAuthorCommandExecute(object? p) => true;

        /// <summary> /// Добавить автора /// </summary>
        public async void OnAddAuthorCommandExecuted(object? p)
        {
            Author author = new Author();
            bool result = _DialogService.Edit(author);
            if (!result)
            {
                return;
            }
            try
            {
                _db.Add(author);
                await _db.SaveChangesAsync();
                Authors.Add(author);
                SelectedAuthor = author;
                _AuthorsViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                _db.Remove(author);
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
        }

        #endregion

        #region команда Удаление книги

        /// <summary> /// Удаление книги /// </summary>
        private ICommand _RemoveBookCommandCommand;

        /// <summary> /// Удаление книги /// </summary>
        public ICommand RemoveBookCommandCommand => _RemoveBookCommandCommand
               ??= new RelayCommand(OnRemoveBookCommandCommandExecuted, CanRemoveBookCommandCommandExecute);

        /// <summary> /// Удаление книги /// </summary>
        public bool CanRemoveBookCommandCommandExecute(object? p) => true;

        /// <summary> /// Удаление книги /// </summary>
        public async void OnRemoveBookCommandCommandExecuted(object? p)
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
            bool result = _DialogService.Edit(book);
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
                if (book.Author == SelectedAuthor)
                {
                    Books.Add(book);
                    SelectedBook = book;
                }
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
            Book book = new Book { Author=SelectedAuthor};
            bool result = _DialogService.Edit(book);
            if (!result) return;
            try
            {
                _db.Add(book);
                await _db.SaveChangesAsync();
                if (book.Author == SelectedAuthor)
                {
                    Books.Add(book);
                    SelectedBook = book;
                }
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
