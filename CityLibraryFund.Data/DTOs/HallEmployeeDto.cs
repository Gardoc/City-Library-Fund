using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class HallEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public DateOnly HireDate { get; set; }
    }
}
