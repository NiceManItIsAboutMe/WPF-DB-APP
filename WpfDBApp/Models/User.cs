using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using WpfDBApp.Database;

namespace WpfDBApp.Models
{
    internal class User
    {
        public int ID { get; set; }

        public string Login { get; set; }

        public byte[] Password { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public float Salary { get; set; }

        public string Phone { get; set; }

        public DateOnly Birthday { get; set; }

        public string Description { get; set; }

        public int DepartmentID { get; set; }

        public int PositionID { get; set; }

    }

    internal class Department
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }

    internal class Position
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSuperuser { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
