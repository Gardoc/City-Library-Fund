using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class VisitListDto
    {
        public int Id { get; set; }

        public string ReaderFullName { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;

        public string EmployeeFullName { get; set; } = string.Empty;

        public DateTime VisitDate { get; set; }
    }
}
