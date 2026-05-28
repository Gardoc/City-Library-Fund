using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class LibraryItemIssueDto
    {
        public int Id { get; set; }

        public string DisplayText { get; set; } = null!;
    }
}
