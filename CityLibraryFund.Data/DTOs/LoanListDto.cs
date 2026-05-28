using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LoanListDto
    {
        public int Id { get; set; }

        public string ReaderFullName { get; set; } = null!;

        public string BookTitle { get; set; } = null!;

        public string InventoryNumber { get; set; } = null!;

        public DateOnly IssueDate { get; set; }

        public DateOnly DueDate { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }

}
