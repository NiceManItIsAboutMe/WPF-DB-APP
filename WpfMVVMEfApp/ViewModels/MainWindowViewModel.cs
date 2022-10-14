using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;

namespace WpfMVVMEfApp.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Заголовок
        private string _Title = "Библиотека";

        public string Title { get => _Title; set => Set(ref _Title, value); }
        #endregion

        #region Выбранная категория

        /// <summary> /// Выбранная категория /// </summary>
        private Category _SelectedCategory;

        /// <summary> /// Выбранная категория /// </summary>
        public Category SelectedCategory { get => _SelectedCategory; set => Set(ref _SelectedCategory, value); }

        #endregion


        #region Категории

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Category> _Categories;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Category> Categories { get => _Categories; set => Set(ref _Categories, value); }

        #endregion



        public MainWindowViewModel(ApplicationContext db)
        {
            Categories = new ObservableCollection<Category>(db.categories.ToArray());
        }

    }
}
