using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Loan")]
public partial class Loan
{
    [Key]
    public int Id { get; set; }

    public int LibraryItemId { get; set; }

    public int ReaderId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Loans")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("LibraryItemId")]
    [InverseProperty("Loans")]
    public virtual LibraryItem LibraryItem { get; set; } = null!;

    [ForeignKey("ReaderId")]
    [InverseProperty("Loans")]
    public virtual Reader Reader { get; set; } = null!;
}
