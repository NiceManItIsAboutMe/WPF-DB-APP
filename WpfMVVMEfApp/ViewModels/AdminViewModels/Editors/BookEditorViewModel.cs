using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels.Editors
{
    internal class BookEditorViewModel : ViewModel
    {
        internal class SelectedCategory
        {
            public bool IsSelected { get; set; } = false;
            public Category Category { get; set; }

            public SelectedCategory(Category category)
            {
                Category = category;
            }
        }

        private readonly IUserDialogService _DialogService;
        #region Книга

        /// <summary> /// Книга /// </summary>
        private Book _Book;

        /// <summary> /// Книга /// </summary>
        public Book Book { get => _Book; set => Set(ref _Book, value); }

        #endregion

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
                    _CategoriesViewSource.View.Refresh();
            }
        }

        #endregion

        #region Файлы

        /// <summary> /// Файлы /// </summary>
        private ObservableCollection<BookFileDescription> _BookFiles;

        /// <summary> /// Файлы /// </summary>
        public ObservableCollection<BookFileDescription> BookFiles { get => _BookFiles; set => Set(ref _BookFiles, value); }

        #endregion

        #region Выбранный файл

        /// <summary> /// Выбранный файл /// </summary>
        private BookFileDescription _SelectedFile;

        /// <summary> /// Выбранный файл /// </summary>
        public BookFileDescription SelectedFile { get => _SelectedFile; set => Set(ref _SelectedFile, value); }

        #endregion

        private CollectionViewSource _CategoriesViewSource;

        /// <summary>/// Отображение пользователей /// </summary>
        public ICollectionView CategoriesView => _CategoriesViewSource?.View;

        #region Категории

        /// <summary> /// Категории /// </summary>
        private ObservableCollection<SelectedCategory> _Categories;

        /// <summary> /// Категории /// </summary>
        public ObservableCollection<SelectedCategory> Categories
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
                            new SortDescription(nameof(SelectedCategory.IsSelected), ListSortDirection.Descending),
                        }
                    };
                    _CategoriesViewSource.Filter += OnCategoriesViewSourceFilter;
                    OnPropertyChanged(nameof(CategoriesView));
                    _CategoriesViewSource.View.Refresh();
                }
            }
        }
        #endregion

        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors { get => _Authors; set => Set(ref _Authors, value); }

        #endregion

        #region команда Загрузка файла книги

        /// <summary> /// Загрузка файла книги /// </summary>
        private ICommand _UploadFileCommand;

        /// <summary> /// Загрузка файла книги /// </summary>
        public ICommand UploadFileCommand => _UploadFileCommand
               ??= new RelayCommand(OnUploadFileCommandExecuted, CanUploadFileCommandExecute);

        /// <summary> /// Загрузка файла книги /// </summary>
        public bool CanUploadFileCommandExecute(object? p) => true;

        /// <summary> /// Загрузка файла книги /// </summary>
        public async void OnUploadFileCommandExecuted(object? p)
        {
            var result = _DialogService.OpenFile("Загрузка книги", out string path);
            if (!result)
                return;
            var file = new BookFile();
            file.File = await File.ReadAllBytesAsync(path);

            var fileName = path.Split(@"\")[^1];
            BookFileDescription bookFileDescription = new BookFileDescription()
            {
                Name = fileName,
                Book = Book,
                File = file
            };

            BookFiles.Add(bookFileDescription);
            SelectedFile = bookFileDescription;
        }

        #endregion

        #region команда Удалить файл
        /// <summary> /// Удалить файл /// </summary>
        private ICommand _DeleteFileCommand;

        /// <summary> /// Удалить файл /// </summary>
        public ICommand DeleteFileCommand => _DeleteFileCommand
               ??= new RelayCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);

        /// <summary> /// Удалить файл /// </summary>
        public bool CanDeleteFileCommandExecute(object? p) => SelectedFile is BookFileDescription;

        /// <summary> /// Удалить файл /// </summary>
        public void OnDeleteFileCommandExecuted(object? p)
        {
            var result = _DialogService.Confirm($"Вы уверены, что хотите удалить файл: {SelectedFile.Name}?", "Удаление файла");
            if (!result) return;

            BookFiles.Remove(SelectedFile);
        }
        #endregion

        #region команда SaveButtonClick SaveButtonClick Изменяем выбранные категории в книге

        /// <summary> /// SaveButtonClick Изменяем выбранные категории в книге /// </summary>
        private ICommand _SaveButtonClickCommand;

        /// <summary> /// SaveButtonClick Изменяем выбранные категории в книге /// </summary>
        public ICommand SaveButtonClickCommand => _SaveButtonClickCommand
               ??= new RelayCommand(OnSaveButtonClickCommandExecuted, CanSaveButtonClickCommandExecute);

        /// <summary> /// Проверям валидаторы/// </summary>
        public bool CanSaveButtonClickCommandExecute(object? p)
        {
            var context = new ValidationContext(Book);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(Book, context, results, true)) return false;

            return true;
        }

        /// <summary> /// SaveButtonClick Изменяем выбранные категории в книге /// </summary>
        public void OnSaveButtonClickCommandExecuted(object? p)
        {
            Book.BookFilesDescription = BookFiles;

            if (Categories == null) return;

            if (Book.Categories == null) Book.Categories = new ObservableCollection<Category>();

            Book.Categories.Clear();
            foreach (var item in Categories.Where(c => c.IsSelected == true))
            {
                Book.Categories.Add(item.Category);
            }

        }

        #endregion

        public BookEditorViewModel(
            Book book,
            IUserDialogService dialogService,
            ObservableCollection<Category>? categories = null,
            ObservableCollection<Author>? authors = null
            )
        {
            _DialogService = dialogService;
            Authors = authors;
            Book = book;
            if (Book.BookFilesDescription != null) BookFiles = new ObservableCollection<BookFileDescription>(book.BookFilesDescription);
            else BookFiles = new ObservableCollection<BookFileDescription>();

            if (categories != null)
            {
                Categories = new ObservableCollection<SelectedCategory>();
                foreach (var item in categories)
                {
                    Categories.Add(new SelectedCategory(item));
                }

                //заполняем коллекцию категорий, к которым относится данная книга
                if (Book.Categories != null)
                {
                    foreach (var item in Book.Categories)
                    {
                        if (Categories.Where(c => c.Category.Id == item.Id).FirstOrDefault() is SelectedCategory category)
                            category.IsSelected = true;
                    }
                    _CategoriesViewSource?.View.Refresh();
                }
            }

        }

        private void OnCategoriesViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is SelectedCategory category) || string.IsNullOrEmpty(SearchFilter)) return;

            if (!category.Category.Name.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}
