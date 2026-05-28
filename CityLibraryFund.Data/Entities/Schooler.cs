using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Schooler")]
public partial class Schooler
{
    [Key]
    public int ReaderId { get; set; }

    [StringLength(100)]
    public string School { get; set; } = null!;

    public int ClassNumber { get; set; }

    [ForeignKey("ReaderId")]
    [InverseProperty("Schooler")]
    public virtual Reader Reader { get; set; } = null!;
}
