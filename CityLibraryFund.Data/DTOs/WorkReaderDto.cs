using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class WorkReaderDto
    {
        public string ReaderFullName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string LibraryName { get; set; } = string.Empty;

        public DateOnly IssueDate { get; set; }

        public DateOnly DueDate { get; set; }
    }
}
