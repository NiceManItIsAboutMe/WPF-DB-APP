using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels.Editors;

namespace WpfMVVMEfApp.ViewModels.MainView
{
    internal class CategoriesViewModel : BooksViewModel
    {
        private CollectionViewSource _CategoriesViewSource;

        public ICollectionView CategoriesView => _CategoriesViewSource?.View;

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
        public Category SelectedCategory { get => _SelectedCategory; set => Set(ref _SelectedCategory, value); }

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
        public async void OnLoadCategoriesDataCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Categories = new ObservableCollection<Category>(await db.Categories.AsNoTracking().ToListAsync());
        }

        #endregion

        #region команда Загрузка книг выбранной категории
        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public override bool CanLoadBooksCommandExecute(object? p) => SelectedCategory is Category;

        /// <summary> /// Загрузка книг выбранной категории /// </summary>
        public override async void OnLoadBooksCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Books = new ObservableCollection<Book>(await db.Books
                .Where(b => b.Categories.Contains(SelectedCategory))
                .Include(b => b.Categories)
                .Include(b => b.Author)
                .Include(b => b.BookFilesDescription)
                .AsNoTracking()
                .ToListAsync());
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

            if (!_DialogService.Confirm($"Вы действительно хотите удалить категорию: {SelectedCategory}", "Удаление"))
                return;

            using var db = await _dbFactory.CreateDbContextAsync();
            db.Remove(SelectedCategory);
            await db.SaveChangesAsync();
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
            using var db = await _dbFactory.CreateDbContextAsync();
            Category category = await db.Categories.Include(c => c.Books).FirstAsync(c => c.Id == SelectedCategory.Id);
            CategoryEditorViewModel vm = new CategoryEditorViewModel(category,
                new ObservableCollection<Book>(await db.Books.ToListAsync()));
            bool result = _DialogService.Edit(vm);
            if (!result) return;

            await db.SaveChangesAsync();
            Categories.Remove(SelectedCategory);
            Categories.Add(category);
            SelectedCategory = category;
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
            using var db = await _dbFactory.CreateDbContextAsync();
            Category category = new Category();
            CategoryEditorViewModel vm = new CategoryEditorViewModel(category,
                new ObservableCollection<Book>(await db.Books.ToListAsync()));
            bool result = _DialogService.Edit(vm);
            if (!result) return;

            db.Add(category);
            await db.SaveChangesAsync();
            Categories.Add(category);
            SelectedCategory = category;
        }

        #endregion

        #region команда Добавление книги
        /// <summary> /// Добавление книги /// </summary>
        public override async void OnAddBookCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Book book = new Book
            {
                Categories = new List<Category> { SelectedCategory }
            };
            BookEditorViewModel vm = new BookEditorViewModel(
                book,
                _DialogService,
                new ObservableCollection<Category>(await db.Categories.ToListAsync()),
                new ObservableCollection<Author>(await db.Authors.ToListAsync()));

            bool result = _DialogService.Edit(vm);
            if (!result) return;

            db.Add(book);
            await db.SaveChangesAsync();
            if (book.Categories.FirstOrDefault(c => c.Id == SelectedCategory.Id) != null)
            {
                SelectedBook = book;
                Books.Add(book);
            }
        }

        #endregion

        #region команда Редактирование книги
        /// <summary> /// Редактирование книги /// </summary>
        public override async void OnEditSelectedBookCommandExecuted(object? p)
        {
            if (SelectedBook == null) return;

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
            db.Update(book);
            await db.SaveChangesAsync();
            Books.Remove(SelectedBook);
            if (book.Categories.FirstOrDefault(c => c.Id == SelectedCategory.Id) != null)
            {
                SelectedBook = book;
                Books.Add(book);
            }
        }

        #endregion

        public CategoriesViewModel(IDbContextFactory<ApplicationDbContext> dbFactory, IUserDialogService dialogService) : base(dbFactory, dialogService)
        {
            _dbFactory = dbFactory;
            _DialogService = dialogService;
        }
        private void OnCategoriesSearchFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Category category) || string.IsNullOrEmpty(CategoriesSearchFilter)) return;

            if (!category.Name.Contains(_CategoriesSearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}
