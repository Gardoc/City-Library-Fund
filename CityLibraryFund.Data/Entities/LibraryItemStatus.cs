using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("LibraryItemStatus")]
[Index("Name", Name = "UQ__LibraryI__737584F6CAADAD0D", IsUnique = true)]
public partial class LibraryItemStatus
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();
}
