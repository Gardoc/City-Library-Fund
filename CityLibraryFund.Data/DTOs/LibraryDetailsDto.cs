using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LibraryDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int HallsCount { get; set; }

        public int EmployeesCount { get; set; }

        public int ReadersCount { get; set; }

        public int LibraryItemsCount { get; set; }

        public int ActiveLoansCount { get; set; }
    }
}
