using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LibraryItemListDto
    {
        public int Id { get; set; }

        public string InventoryNumber { get; set; } = string.Empty;

        public string WorkTitle { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public int PublishYear { get; set; }

        public string LibraryName { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public bool OnlyReadingRoom { get; set; }
    }
}
