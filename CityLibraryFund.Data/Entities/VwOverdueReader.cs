using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Keyless]
public partial class VwOverdueReader
{
    public int ReaderId { get; set; }

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string? Patronymic { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public int? OverdueDays { get; set; }
}
