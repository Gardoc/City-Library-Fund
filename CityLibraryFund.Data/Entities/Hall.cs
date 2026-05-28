using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Hall")]
public partial class Hall
{
    [Key]
    public int Id { get; set; }

    public int LibraryId { get; set; }

    public int HallTypeId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int? Floor { get; set; }

    [InverseProperty("Hall")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [ForeignKey("HallTypeId")]
    [InverseProperty("Halls")]
    public virtual HallType HallType { get; set; } = null!;

    [ForeignKey("LibraryId")]
    [InverseProperty("Halls")]
    public virtual Library Library { get; set; } = null!;

    [InverseProperty("Hall")]
    public virtual ICollection<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();

    [InverseProperty("Hall")]
    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
