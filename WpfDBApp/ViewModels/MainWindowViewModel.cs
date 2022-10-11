using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfDBApp.Base.ViewModels;
using WpfDBApp.Database;
using WpfDBApp.Models;

namespace WpfDBApp.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region поля
        public ObservableCollection<Department> Departments { get; }
        public ObservableCollection<Position> Positions { get; }

        private Department _SelectedDepartment;
        public Department SelectedDepartment
        {
            get => _SelectedDepartment;
            set => Set(ref _SelectedDepartment, value);
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
                Password = SHA512.HashData(ASCIIEncoding.ASCII.GetBytes($"password{user_index}")),
                Phone = $"phone{user_index}",
                Description = $"description{user_index++}"
                
            });
            var departments = Enumerable.Range(1, 10).Select(i => new Department
            {
                ID = i,
                Name = $"Отдел {i}",
                Users = new ObservableCollection<User>(users)
            });

            var positions = Enumerable.Range(1, 10).Select(i => new Position
            {
                ID = i,
                Name = $"Должность {i}",
                IsSuperuser=false,
                Users = new ObservableCollection<User>(users)
            });*/
            using var db = new Connection();
            using var connection = db.GetConnection();
            using var selectUsers = new NpgsqlCommand("SELECT * FROM users WHERE departmentid=$1", connection);
            using var readerUsers = selectUsers.ExecuteReader();
            using var selectDepartments = new NpgsqlCommand("SELECT * FROM users", connection);
            using var readerDepartments = selectDepartments.ExecuteReader();
            using var selectPositions = new NpgsqlCommand("SELECT * FROM users", connection);
            using var readerPositions = selectPositions.ExecuteReader();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(selectUsers);
            adapter.Fill();
            
            Departments = new ObservableCollection<Department>();
            while (readerDepartments.Read())
            {
                Departments.Add(new Department()
                {
                    ID = readerDepartments.GetInt32(0),
                    Name = readerDepartments.GetString(1),
                    Description = readerDepartments.GetString(2)
                });
            }
            Positions = new ObservableCollection<Department>(departments);
            var users = Enumerable.Range(1, 10).Select(i => new User
            {
                Name = $"Имя {user_index}",
                Surname = $"Фамилия {user_index}",
                Patronymic = $"Отчество {user_index}",
                Birthday = DateOnly.FromDateTime(DateTime.Now),
                Salary = 40000,
                Login = $"login{user_index}",
                Password = SHA512.HashData(ASCIIEncoding.ASCII.GetBytes($"password{user_index}")),
                Phone = $"phone{user_index}",
                Description = $"description{user_index++}"
            });
            /*foreach (var item in positions)
            {
                using var cmd = new NpgsqlCommand("INSERT INTO positions (name,issuperuser) VALUES ($1,$2)", connection)
                {
                    Parameters =
                    {
                        new(){Value=item.Name},
                        new(){Value=item.IsSuperuser}
                    }
                };
                cmd.ExecuteNonQuery();
            }
                foreach (var item in Departments)
            {
                using var cmd = new NpgsqlCommand("INSERT INTO departments (name) VALUES ($1)", connection)
                {
                    Parameters =
                    {
                        new(){Value=item.Name}
                    }
                };
                cmd.ExecuteNonQuery();
                foreach (var user in item.Users)
                {
                    using var cmd2 = new NpgsqlCommand("INSERT INTO public.users" +
                        "(name, surname, patronymic, description, login, password, departmentid, salary, birthday, phone,positionid)" +
                        "VALUES ($1, $2, $3, $4,$5, $6,$7,$8,$9,$10, $11);", connection)
                    {
                        Parameters =
                    {
                    new(){Value=user.Name},
                    new(){Value=user.Surname},
                    new(){Value=user.Patronymic},
                    new(){Value=user.Description},
                    new(){Value=user.Login},
                    new(){Value=user.Password},
                    new(){Value=item.ID},
                    new(){Value=user.Salary},
                    new(){Value=user.Birthday},
                    new(){Value=user.Phone},
                    new(){Value=item.ID}
                    }
                    };
                    cmd2.ExecuteNonQuery();
                }
            }*/
        }
    }
}
