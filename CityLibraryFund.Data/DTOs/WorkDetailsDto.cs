using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class WorkDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string WorkType { get; set; } = string.Empty;

        public string Authors { get; set; } = string.Empty;

        public int? YearWritten { get; set; }

        public string Language { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int EditionsCount { get; set; }

        public int CopiesCount { get; set; }

        public string? LatestEdition { get; set; }
    }
}
