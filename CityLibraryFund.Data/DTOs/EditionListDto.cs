using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EditionListDto
    {
        public int Id { get; set; }

        public string WorkTitle { get; set; } = string.Empty;

        public string Authors { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public int PublishYear { get; set; }

        public string Isbn { get; set; } = string.Empty;

        public int? EditionNumber { get; set; }

        public int CopiesCount { get; set; }
    }
}
