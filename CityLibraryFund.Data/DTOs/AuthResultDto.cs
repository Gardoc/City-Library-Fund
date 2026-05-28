using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int EmployeeId { get; set; }

        public int HallId { get; set; }

        public string HallName { get; set; } = string.Empty;

        public int LibraryId { get; set; }

        public string LibraryName { get; set; } = string.Empty;
    }
}
