using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class ChangePasswordDto
    {
        public int EmployeeId { get; set; }

        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}