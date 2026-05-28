using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Edition")]
[Index("Isbn", Name = "UQ__Edition__447D36EAB7CC33A2", IsUnique = true)]
public partial class Edition
{
    [Key]
    public int Id { get; set; }

    public int WorkId { get; set; }

    [StringLength(100)]
    public string Publisher { get; set; } = null!;

    public int PublishYear { get; set; }

    [Column("ISBN")]
    [StringLength(30)]
    public string? Isbn { get; set; }

    public int? PageCount { get; set; }

    public int? EditionNumber { get; set; }

    [InverseProperty("Edition")]
    public virtual ICollection<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();

    [ForeignKey("WorkId")]
    [InverseProperty("Editions")]
    public virtual Work Work { get; set; } = null!;
}
