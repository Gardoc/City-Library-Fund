using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Library")]
[Index("Phone", Name = "UQ__Library__5C7E359E081A6735", IsUnique = true)]
public partial class Library
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string Address { get; set; } = null!;

    [StringLength(20)]
    public string Phone { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("Library")]
    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();

    [InverseProperty("Library")]
    public virtual ICollection<Reader> Readers { get; set; } = new List<Reader>();
}
