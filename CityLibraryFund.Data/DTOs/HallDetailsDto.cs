using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class HallDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;

        public string HallType { get; set; } = string.Empty;

        public int? Floor { get; set; }

        public int EmployeesCount { get; set; }

        public int LibraryItemsCount { get; set; }

        public int ActiveLoansCount { get; set; }

        public int VisitsCount { get; set; }
    }
}
