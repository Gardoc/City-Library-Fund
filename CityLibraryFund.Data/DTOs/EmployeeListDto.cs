using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EmployeeListDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}