using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LibraryItemDetailsDto
    {
        public int Id { get; set; }

        public string InventoryNumber { get; set; } = string.Empty;

        public string WorkTitle { get; set; } = string.Empty;

        public string Authors { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public int PublishYear { get; set; }

        public string? Isbn { get; set; }

        public string LibraryName { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public int RackNumber { get; set; }

        public int ShelfNumber { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool OnlyReadingRoom { get; set; }

        public int? LoanDays { get; set; }

        public DateOnly ArrivalDate { get; set; }

        public DateOnly? WriteOffDate { get; set; }

        public decimal? Price { get; set; }

        public bool IsWrittenOff { get; set; }

        public bool IsIssued { get; set; }
    }
}
