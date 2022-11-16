using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels.AdminViewModels.Editors;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class AuthorsViewModel : BooksViewModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbFactory;

        private IUserDialogService _DialogService;

        private CollectionViewSource _AuthorsViewSource;

        public ICollectionView AuthorsView => _AuthorsViewSource?.View;

        #region Поиск авторов

        /// <summary> /// Поиск авторов /// </summary>
        private string _AuthorsSearchFilter;

        /// <summary> /// Поиск авторов /// </summary>
        public string AuthorsSearchFilter
        {
            get => _AuthorsSearchFilter;
            set
            {
                if (Set(ref _AuthorsSearchFilter, value))
                    _AuthorsViewSource.View.Refresh();
            }
        }

        #endregion

        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors
        {
            get => _Authors; set
            {
                if (Set(ref _Authors, value))
                {
                    _AuthorsViewSource = new CollectionViewSource
                    {
                        Source = Authors,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(Author.Surname),ListSortDirection.Ascending),
                        }
                    };
                    _AuthorsViewSource.Filter += OnAuthorsSearchFilter;
                    OnPropertyChanged(nameof(AuthorsView));
                    _AuthorsViewSource.View.Refresh();
                }

            }
        }
        #endregion

        #region Выбранный автор

        /// <summary> /// Выбранный автор /// </summary>
        private Author _SelectedAuthor;

        /// <summary> /// Выбранный автор /// </summary>
        public Author SelectedAuthor { get => _SelectedAuthor; set => Set(ref _SelectedAuthor, value); }

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
        public async void OnLoadAuthorsDataCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Authors = new ObservableCollection<Author>(await db.Authors.AsNoTracking().ToListAsync());
        }

        #endregion

        #region команда Загрузка книг выбранного автора
        /// <summary> /// Загрузка книг выбранного автора /// </summary>
        public override bool CanLoadBooksCommandExecute(object? p) => SelectedAuthor is Author;
        /// <summary> /// Загрузка книг выбранного автора /// </summary>
        public override async void OnLoadBooksCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Books = new ObservableCollection<Book>(await db.Books
                .Where(b => b.Author.Id == SelectedAuthor.Id)
                .Include(b => b.Categories)
                .Include(b => b.Author)
                .Include(b => b.BookFilesDescription)
                .AsNoTracking()
                .ToListAsync());
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

            using var db = await _dbFactory.CreateDbContextAsync();
            if (!_DialogService.Confirm($"Вы действительно хотите удалить автора: {SelectedAuthor}\n При удалении автора удалятся так же и все его книги!!!", "Удаление"))
                return;
            db.Remove(SelectedAuthor);
            await db.SaveChangesAsync();
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
            using var db = await _dbFactory.CreateDbContextAsync();
            Author author = await db.Authors.FirstAsync(a => a.Id == SelectedAuthor.Id);
            bool result = _DialogService.Edit(author);
            if (!result) return;
            try
            {
                db.Update(author);
                await db.SaveChangesAsync();
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
            using var db = await _dbFactory.CreateDbContextAsync();
            bool result = _DialogService.Edit(author);
            if (!result)
            {
                return;
            }
            try
            {
                db.Add(author);
                await db.SaveChangesAsync();
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

        #region команда Редактирование книги
        /// <summary> /// Редактирование книги /// </summary>
        public override bool CanEditSelectedBookCommandExecute(object? p) => SelectedBook is Book;

        /// <summary> /// Редактирование книги /// </summary>
        public override async void OnEditSelectedBookCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();

            Book book = await db.Books.Include(b => b.Categories).Include(b => b.Author).Include(b => b.BookFilesDescription)
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
            if (book.Author.Id == SelectedAuthor.Id)
            {
                Books.Add(book);
                SelectedBook = book;
            }
            _BooksViewSource.View.Refresh();
        }

        #endregion

        #region команда Добавить книгу
        /// <summary> /// Добавить книгу /// </summary>
        public override async void OnAddBookCommandExecuted(object? p)
        {
            Book book = new Book { Author = SelectedAuthor };
            using var db = await _dbFactory.CreateDbContextAsync();
            BookEditorViewModel vm = new BookEditorViewModel(
                book,
                _DialogService,
                new ObservableCollection<Category>(await db.Categories.ToListAsync()),
                new ObservableCollection<Author>(await db.Authors.ToListAsync()));

            bool result = _DialogService.Edit(vm);
            if (!result) return;

            db.Add(book);
            await db.SaveChangesAsync();
            if (book.Author.Id == SelectedAuthor.Id)
            {
                Books.Add(book);
                SelectedBook = book;
            }
        }

        #endregion


        public AuthorsViewModel(IDbContextFactory<ApplicationDbContext> dbFactory, IUserDialogService dialogService) : base(dbFactory, dialogService)
        {
            _dbFactory = dbFactory;
            _DialogService = dialogService;
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
