using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Teacher")]
public partial class Teacher
{
    [Key]
    public int ReaderId { get; set; }

    [StringLength(100)]
    public string University { get; set; } = null!;

    [StringLength(100)]
    public string Department { get; set; } = null!;

    [ForeignKey("ReaderId")]
    [InverseProperty("Teacher")]
    public virtual Reader Reader { get; set; } = null!;
}
