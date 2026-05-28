using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LoanAnalyticsDto
    {
        public string ReaderFullName { get; set; } = string.Empty;

        public string WorkTitle { get; set; } = string.Empty;

        public string EditionTitle { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;

        public DateOnly IssueDate { get; set; }

        public DateOnly? ReturnDate { get; set; }
    }
}
