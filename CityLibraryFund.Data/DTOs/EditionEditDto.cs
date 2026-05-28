using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EditionEditDto
    {
        public int Id { get; set; }

        public int WorkId { get; set; }

        public string Publisher { get; set; } = string.Empty;

        public int PublishYear { get; set; }

        public string? Isbn { get; set; }

        public int? PageCount { get; set; }

        public int? EditionNumber { get; set; }
    }
}
