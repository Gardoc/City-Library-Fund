using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class CreateVisitDto
    {
        public int ReaderId { get; set; }

        public int HallId { get; set; }

        public DateTime VisitDate { get; set; }
    }
}
