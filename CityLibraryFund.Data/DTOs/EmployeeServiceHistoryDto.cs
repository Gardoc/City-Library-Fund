using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EmployeeServiceHistoryDto
    {
        public string ReaderName { get; set; } = string.Empty;

        public string WorkTitle { get; set; } = string.Empty;

        public DateOnly LoanDate { get; set; }

        public DateOnly DueDate { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public bool IsReturned { get; set; }
    }
}
