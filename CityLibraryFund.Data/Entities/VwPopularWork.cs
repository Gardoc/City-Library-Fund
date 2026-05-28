using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Keyless]
public partial class VwPopularWork
{
    public int WorkId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public int? LoanCount { get; set; }
}
