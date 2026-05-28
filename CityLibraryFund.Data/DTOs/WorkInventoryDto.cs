using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class WorkInventoryDto
    {
        public string InventoryNumber { get; set; } = string.Empty;

        public string EditionInfo { get; set; } = string.Empty;

        public string LibraryName { get; set; } = string.Empty;
    }
}
