using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Scientist")]
public partial class Scientist
{
    [Key]
    public int ReaderId { get; set; }

    [StringLength(150)]
    public string Organization { get; set; } = null!;

    [StringLength(200)]
    public string ScientificTopic { get; set; } = null!;

    [ForeignKey("ReaderId")]
    [InverseProperty("Scientist")]
    public virtual Reader Reader { get; set; } = null!;
}
