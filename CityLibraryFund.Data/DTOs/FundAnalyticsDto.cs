using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class FundAnalyticsDto
    {
        public string InventoryNumber { get; set; } = string.Empty;

        public string WorkTitle { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public string ShelfCode { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateOnly ArrivalDate { get; set; }

        public DateOnly? WriteOffDate { get; set; }
    }
}
