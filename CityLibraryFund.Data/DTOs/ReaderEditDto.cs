using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class ReaderEditDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Patronymic { get; set; }

        public DateOnly BirthDate { get; set; }

        public int ReaderTypeId { get; set; }

        public int LibraryId { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public string? University { get; set; }

        public string? Faculty { get; set; }

        public int? Course { get; set; }

        public string? GroupName { get; set; }

        public string? Department { get; set; }

        public string? Organization { get; set; }

        public string? ScientificTopic { get; set; }

        public string? School { get; set; }

        public int? ClassNumber { get; set; }
    }
}
