using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class IssueLoanDto
    {
        public int ReaderId { get; set; }

        public int LibraryItemId { get; set; }

        public DateOnly IssueDate { get; set; }

        public DateOnly DueDate { get; set; }
    }
}
