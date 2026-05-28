using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.DTOs
{
    public class OperationResultDto
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
