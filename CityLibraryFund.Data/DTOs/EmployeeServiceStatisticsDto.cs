using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class EmployeeServiceStatisticsDto
    {
        public int ReadersCount { get; set; }

        public int LoansCount { get; set; }

        public int ReturnedCount { get; set; }

        public int ActiveLoansCount { get; set; }
    }
}
