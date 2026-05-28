using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("ReaderType")]
[Index("Name", Name = "UQ__ReaderTy__737584F66822BE2D", IsUnique = true)]
public partial class ReaderType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("ReaderType")]
    public virtual ICollection<Reader> Readers { get; set; } = new List<Reader>();
}
