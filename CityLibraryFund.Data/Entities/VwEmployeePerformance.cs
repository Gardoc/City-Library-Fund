using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Keyless]
public partial class VwEmployeePerformance
{
    public int EmployeeId { get; set; }

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    public int? ReadersServed { get; set; }
}
