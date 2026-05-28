using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class ReaderAnalyticsDto
    {
        public string FullName { get; set; } = string.Empty;

        public string ReaderType { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? AdditionalInfo { get; set; }
    }
}
