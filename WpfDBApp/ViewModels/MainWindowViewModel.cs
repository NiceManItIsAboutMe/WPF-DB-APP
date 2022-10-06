using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfDBApp.Base.ViewModels;
using WpfDBApp.Models;

namespace WpfDBApp.ViewModels
{
    internal class MainWindowViewModel:ViewModel
    {
        #region поля
        public ObservableCollection<Department> Departments { get; }

        private Department _SelectedDepartment;
        public Department SelectedDepartment
        {
            get => _SelectedDepartment;
            set=>Set(ref _SelectedDepartment, value);
        }
        #endregion
        public MainWindowViewModel()
        {
            // инициализирование коллекций
            var user_index = 3;
            var users = Enumerable.Range(1, 10).Select(i => new User
            {
                Name = $"Имя {user_index}",
                Surname = $"Фамилия {user_index}",
                Patronymic = $"Отчество {user_index}",
                Birthday = DateOnly.FromDateTime(DateTime.Now),
                Salary = 40000,
                Login = $"login{user_index}",
                Password = $"password{user_index}",
                Phone = $"phone{user_index}",
                IsSuperUser = false,
                Id = user_index++
            });
            var departments = Enumerable.Range(1, 10).Select(i => new Department
            {
                Name = $"Отдел {i}",
                Users = new ObservableCollection<User>(users)
            });
            Departments = new ObservableCollection<Department>(departments);

        }
    }
}
