using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("LibraryItem")]
[Index("InventoryNumber", Name = "UQ__LibraryI__D6D65CC8C2EA23F3", IsUnique = true)]
public partial class LibraryItem
{
    [Key]
    public int Id { get; set; }

    public int EditionId { get; set; }

    public int HallId { get; set; }

    public int StatusId { get; set; }

    [StringLength(50)]
    public string InventoryNumber { get; set; } = null!;

    public int RackNumber { get; set; }

    public int ShelfNumber { get; set; }

    public bool OnlyReadingRoom { get; set; }

    public int? LoanDays { get; set; }

    public DateOnly ArrivalDate { get; set; }

    public DateOnly? WriteOffDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Price { get; set; }

    [ForeignKey("EditionId")]
    [InverseProperty("LibraryItems")]
    public virtual Edition Edition { get; set; } = null!;

    [ForeignKey("HallId")]
    [InverseProperty("LibraryItems")]
    public virtual Hall Hall { get; set; } = null!;

    [InverseProperty("LibraryItem")]
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    [ForeignKey("StatusId")]
    [InverseProperty("LibraryItems")]
    public virtual LibraryItemStatus Status { get; set; } = null!;
}
