using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("WorkType")]
[Index("Name", Name = "UQ__WorkType__737584F64A971A04", IsUnique = true)]
public partial class WorkType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("WorkType")]
    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
