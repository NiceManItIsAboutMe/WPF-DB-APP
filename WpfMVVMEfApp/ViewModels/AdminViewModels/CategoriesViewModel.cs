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

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class CategoriesViewModel : ViewModel
    {
        private BooksDbContext _db;

        private IUserDialogService _DialogService;

        private CollectionViewSource _CategoriesViewSource;

        public ICollectionView CategoriesView => _CategoriesViewSource?.View;

        private CollectionViewSource _BooksViewSource;

        public ICollectionView BooksView => _BooksViewSource?.View;

        #region Поиск категорий

        /// <summary> /// Поиск категорий /// </summary>
        private string _CategoriesSearchFilter;

        /// <summary> /// Поиск категорий /// </summary>
        public string CategoriesSearchFilter
        {
            get => _CategoriesSearchFilter;
            set
            {
                if (Set(ref _CategoriesSearchFilter, value))
                    _CategoriesViewSource.View.Refresh();
            }
        }

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


        #region Категории

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Category> _Categories;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Category> Categories
        {
            get => _Categories;
            set
            {
                if (Set(ref _Categories, value))
                {
                    _CategoriesViewSource = new CollectionViewSource
                    {
                        Source = Categories,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(Category.Name), ListSortDirection.Ascending),
                        }
                    };
                    _CategoriesViewSource.Filter += OnCategoriesSearchFilter;
                    OnPropertyChanged(nameof(CategoriesView));
                    _CategoriesViewSource.View.Refresh();
                }
            }
        }

        #endregion

        #region Выбранная категория

        /// <summary> /// Выбранная категория /// </summary>
        private Category _SelectedCategory;

        /// <summary> /// Выбранная категория /// </summary>
        public Category SelectedCategory
        {
            get => _SelectedCategory;
            set
            {
                Set(ref _SelectedCategory, value);
                if (CanLoadBooksSelectedCategoriesCommandExecute(null))
                    OnLoadBooksSelectedCategoriesCommandExecuted(null);
            }
        }

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


        #region команда Загрузка категорий

        /// <summary> /// Загрузка категорий /// </summary>
        private ICommand _LoadCategoriesDataCommand;

        /// <summary> /// Загрузка категорий /// </summary>
        public ICommand LoadCategoriesDataCommand => _LoadCategoriesDataCommand
               ??= new RelayCommand(OnLoadCategoriesDataCommandExecuted, CanLoadCategoriesDataCommandExecute);

        /// <summary> /// Загрузка категорий /// </summary>
        public bool CanLoadCategoriesDataCommandExecute(object? p) => true;

        /// <summary> /// Загрузка категорий /// </summary>
        public void OnLoadCategoriesDataCommandExecuted(object? p)
        {
            Categories = new ObservableCollection<Category>(_db.Categories.AsNoTracking());
        }

        #endregion


        #region команда Загрузка книг выбранной категории

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        private ICommand _LoadBooksSelectedCategoriesCommand;

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public ICommand LoadBooksSelectedCategoriesCommand => _LoadBooksSelectedCategoriesCommand
               ??= new RelayCommand(OnLoadBooksSelectedCategoriesCommandExecuted, CanLoadBooksSelectedCategoriesCommandExecute);

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public bool CanLoadBooksSelectedCategoriesCommandExecute(object? p)
        {
            if (SelectedCategory == null) return false;
            return true;
        }

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public void OnLoadBooksSelectedCategoriesCommandExecuted(object? p)
        {
            Books = new ObservableCollection<Book>(_db.Books.Where(b => b.Categories.Contains(SelectedCategory))
                .Include(b=>b.Categories).Include(b => b.Author).AsNoTracking());
        }

        #endregion


        #region команда Удаление категории

        /// <summary> /// Удаление категории /// </summary>
        private ICommand _RemoveSelectedCategoryCommand;

        /// <summary> /// Удаление категории /// </summary>
        public ICommand RemoveSelectedCategoryCommand => _RemoveSelectedCategoryCommand
               ??= new RelayCommand(OnRemoveSelectedCategoryCommandExecuted, CanRemoveSelectedCategoryCommandExecute);

        /// <summary> /// Удаление категории /// </summary>
        public bool CanRemoveSelectedCategoryCommandExecute(object? p) => true;

        /// <summary> /// Удаление категории /// </summary>
        public async void OnRemoveSelectedCategoryCommandExecuted(object? p)
        {
            if (SelectedCategory == null) return;

            Category category = await _db.Categories.FirstAsync(c => c.Id == SelectedCategory.Id);
            if (!_DialogService.Confirm($"Вы действительно хотите удалить категорию: {SelectedCategory}", "Удаление"))
                return;
            _db.Remove(category);
            await _db.SaveChangesAsync();
            Categories.Remove(SelectedCategory);
        }

        #endregion

        #region команда Редактирование категории

        /// <summary> /// Редактирование категории /// </summary>
        private ICommand _EditSelectedCategoryCommand;

        /// <summary> /// Редактирование категории /// </summary>
        public ICommand EditSelectedCategoryCommand => _EditSelectedCategoryCommand
               ??= new RelayCommand(OnEditSelectedCategoryCommandExecuted, CanEditSelectedCategoryCommandExecute);

        /// <summary> /// Редактирование категории /// </summary>
        public bool CanEditSelectedCategoryCommandExecute(object? p) => SelectedCategory is Category;

        /// <summary> /// Редактирование категории /// </summary>
        public async void OnEditSelectedCategoryCommandExecuted(object? p)
        {
            Category category = await _db.Categories.Include(c=>c.Books).FirstAsync(c=>c.Id==SelectedCategory.Id);
            bool result = _DialogService.Edit(category);
            if (!result)
            {
                // перестаем остлеживать данную сущность, иначе при следующем входе в редактор мы получим изменненую сущность, которая была закэширована EF
                _db.Entry(category).State = EntityState.Detached;
                return;
            }
            try
            {
                _db.Update(category);
                await _db.SaveChangesAsync();
                _db.Entry(category).State = EntityState.Detached;
                Categories.Remove(SelectedCategory);
                Categories.Add(category);
                SelectedCategory = category;
                _CategoriesViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
        }

        #endregion

        #region команда Добавление категории

        /// <summary> /// Добавление категории /// </summary>
        private ICommand _AddCategoryCommand;

        /// <summary> /// Добавление категории /// </summary>
        public ICommand AddCategoryCommand => _AddCategoryCommand
               ??= new RelayCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);

        /// <summary> /// Добавление категории /// </summary>
        public bool CanAddCategoryCommandExecute(object? p) => true;

        /// <summary> /// Добавление категории /// </summary>
        public async void OnAddCategoryCommandExecuted(object? p)
        {
            Category category = new Category();
            bool result = _DialogService.Edit(category);
            if (!result)
            {
                return;
            }
            try
            {
                _db.Add(category);
                await _db.SaveChangesAsync();
                Categories.Add(category);
                SelectedCategory = category;
                _CategoriesViewSource.View.Refresh();
            }
            catch(Exception ex)
            {
                _db.Remove(category);
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
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

        #region команда Добавление книги

        /// <summary> /// Добавление книги /// </summary>
        private ICommand _AddBookCommand;

        /// <summary> /// Добавление книги /// </summary>
        public ICommand AddBookCommand => _AddBookCommand
               ??= new RelayCommand(OnAddBookCommandExecuted, CanAddBookCommandExecute);

        /// <summary> /// Добавление книги /// </summary>
        public bool CanAddBookCommandExecute(object? p) => true;

        /// <summary> /// Добавление книги /// </summary>
        public async void OnAddBookCommandExecuted(object? p)
        {
            Book book = new Book
            {
                Categories=new List<Category> { SelectedCategory}
            };
            bool result = _DialogService.Edit(book);
            if (!result) return;
            try
            {
                _db.Add(book);
                await _db.SaveChangesAsync();
                if (book.Categories.Contains(SelectedCategory))
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

        #region команда Редактирование книги

        /// <summary> /// Редактирование книги /// </summary>
        private ICommand _EditSelectedBookCommand;

        /// <summary> /// Редактирование книги /// </summary>
        public ICommand EditSelectedBookCommand => _EditSelectedBookCommand
               ??= new RelayCommand(OnEditSelectedBookCommandExecuted, CanEditSelectedBookCommandExecute);

        /// <summary> /// Редактирование книги /// </summary>
        public bool CanEditSelectedBookCommandExecute(object? p) => true;

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
                if (book.Categories.Contains(SelectedCategory))
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

        public CategoriesViewModel(BooksDbContext db, IUserDialogService dialogService)
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

        private void OnCategoriesSearchFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Category category) || string.IsNullOrEmpty(CategoriesSearchFilter)) return;

            if (!category.Name.Contains(_CategoriesSearchFilter,StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}
