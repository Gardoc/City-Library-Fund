using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Work")]
public partial class Work
{
    [Key]
    public int Id { get; set; }

    public int WorkTypeId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public int? YearWritten { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [InverseProperty("Work")]
    public virtual ICollection<Edition> Editions { get; set; } = new List<Edition>();

    [ForeignKey("WorkTypeId")]
    [InverseProperty("Works")]
    public virtual WorkType WorkType { get; set; } = null!;

    [ForeignKey("WorkId")]
    [InverseProperty("Works")]
    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}
