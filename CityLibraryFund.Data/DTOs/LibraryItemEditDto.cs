using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LibraryItemEditDto
    {
        public int Id { get; set; }

        public int EditionId { get; set; }

        public int HallId { get; set; }

        public string InventoryNumber { get; set; }
            = string.Empty;

        public int RackNumber { get; set; }

        public int ShelfNumber { get; set; }

        public bool OnlyReadingRoom { get; set; }

        public int? LoanDays { get; set; }

        public DateOnly ArrivalDate { get; set; }

        public DateOnly? WriteOffDate { get; set; }

        public decimal? Price { get; set; }
    }
}
