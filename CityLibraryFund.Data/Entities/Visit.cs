using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Visit")]
public partial class Visit
{
    [Key]
    public int Id { get; set; }

    public int ReaderId { get; set; }

    public int EmployeeId { get; set; }

    public int HallId { get; set; }

    public DateTime VisitDate { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Visits")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("HallId")]
    [InverseProperty("Visits")]
    public virtual Hall Hall { get; set; } = null!;

    [ForeignKey("ReaderId")]
    [InverseProperty("Visits")]
    public virtual Reader Reader { get; set; } = null!;
}
