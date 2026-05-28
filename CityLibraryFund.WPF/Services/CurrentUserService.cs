using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.WPF.Services
{
    public static class CurrentUserService
    {
        public static int EmployeeId { get; set; }

        public static string FullName { get; set; } = string.Empty;

        public static string Role { get; set; } = string.Empty;

        public static int HallId { get; set; }

        public static string HallName { get; set; } = string.Empty;

        public static int LibraryId { get; set; }

        public static string LibraryName { get; set; } = string.Empty;
    }
}
