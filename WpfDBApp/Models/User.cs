using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace WpfDBApp.Models
{
    internal class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public float Salary { get; set; }

        public bool IsSuperUser { get; set; }

        public string Phone { get; set; }

        public DateOnly Birthday { get; set; }

    }

    internal class Department
    {
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
