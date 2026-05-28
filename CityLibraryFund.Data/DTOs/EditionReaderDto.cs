using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EditionReaderDto
    {
        public string ReaderName { get; set; } = string.Empty;

        public string ReaderType { get; set; } = string.Empty;

        public string InventoryNumber { get; set; } = string.Empty;

        public DateOnly IssueDate { get; set; }

        public DateOnly DueDate { get; set; }

        public int DaysLeft { get; set; }
    }
}
