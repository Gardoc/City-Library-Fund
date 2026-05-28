using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class ReaderListDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string ReaderType { get; set; } = null!;

        public string? Phone { get; set; }

        public string LibraryName { get; set; } = null!;

        public DateOnly RegistrationDate { get; set; }

        public bool IsActive { get; set; }
    }
}
