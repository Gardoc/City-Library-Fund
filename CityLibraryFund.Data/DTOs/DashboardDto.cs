using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class DashboardDto
    {
        public int ReadersCount { get; set; }

        public int BooksCount { get; set; }

        public int ActiveLoansCount { get; set; }

        public int OverdueCount { get; set; }
    }
}
