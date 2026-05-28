using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class WorkEditDto
    {
        public int Id { get; set; }

        public int WorkTypeId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int? YearWritten { get; set; }

        public string? Description { get; set; }

        public string? Language { get; set; }

        public List<int> AuthorIds { get; set; } = new();
    }
}