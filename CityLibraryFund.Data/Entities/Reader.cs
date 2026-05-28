using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Reader")]
public partial class Reader
{
    [Key]
    public int Id { get; set; }

    public int LibraryId { get; set; }

    public int ReaderTypeId { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string? Patronymic { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly RegistrationDate { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("LibraryId")]
    [InverseProperty("Readers")]
    public virtual Library Library { get; set; } = null!;

    [InverseProperty("Reader")]
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    [ForeignKey("ReaderTypeId")]
    [InverseProperty("Readers")]
    public virtual ReaderType ReaderType { get; set; } = null!;

    [InverseProperty("Reader")]
    public virtual Schooler? Schooler { get; set; }

    [InverseProperty("Reader")]
    public virtual Scientist? Scientist { get; set; }

    [InverseProperty("Reader")]
    public virtual Student? Student { get; set; }

    [InverseProperty("Reader")]
    public virtual Teacher? Teacher { get; set; }

    [InverseProperty("Reader")]
    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
