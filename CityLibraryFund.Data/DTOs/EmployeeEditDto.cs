using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EmployeeEditDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Patronymic { get; set; }

        public string Position { get; set; } = string.Empty;

        public DateOnly HireDate { get; set; }

        public int EmployeeRoleId { get; set; }

        public int HallId { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}