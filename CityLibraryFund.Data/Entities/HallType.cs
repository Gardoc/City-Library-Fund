using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("HallType")]
[Index("Name", Name = "UQ__HallType__737584F6ADF8FDF0", IsUnique = true)]
public partial class HallType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("HallType")]
    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
