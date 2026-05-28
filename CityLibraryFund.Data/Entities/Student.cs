using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Student")]
public partial class Student
{
    [Key]
    public int ReaderId { get; set; }

    [StringLength(100)]
    public string University { get; set; } = null!;

    [StringLength(100)]
    public string Faculty { get; set; } = null!;

    public int Course { get; set; }

    [StringLength(20)]
    public string GroupName { get; set; } = null!;

    [ForeignKey("ReaderId")]
    [InverseProperty("Student")]
    public virtual Reader Reader { get; set; } = null!;
}
