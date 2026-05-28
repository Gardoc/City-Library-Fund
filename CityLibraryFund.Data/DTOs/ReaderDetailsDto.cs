using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class ReaderDetailsDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string ReaderType { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public DateOnly RegistrationDate { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string LibraryName { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? University { get; set; }

        public string? Faculty { get; set; }

        public int? Course { get; set; }

        public string? GroupName { get; set; }

        public string? Department { get; set; }

        public string? Organization { get; set; }

        public string? ScientificTopic { get; set; }

        public string? School { get; set; }

    }
}
