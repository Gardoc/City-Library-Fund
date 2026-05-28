using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class AuthorDetailsDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public DateOnly? BirthDate { get; set; }

        public string Country { get; set; } = string.Empty;

        public int WorksCount { get; set; }

        public int CopiesCount { get; set; }

        public string? LatestWork { get; set; }
    }
}
