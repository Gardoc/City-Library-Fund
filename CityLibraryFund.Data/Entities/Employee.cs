using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Employee")]
[Index("Login", Name = "UQ__Employee__5E55825BC3D72066", IsUnique = true)]
public partial class Employee
{
    [Key]
    public int Id { get; set; }

    public int HallId { get; set; }

    public int EmployeeRoleId { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string? Patronymic { get; set; }

    [StringLength(50)]
    public string Position { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    [StringLength(50)]
    public string Login { get; set; } = null!;

    [StringLength(100)]
    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    [ForeignKey("EmployeeRoleId")]
    [InverseProperty("Employees")]
    public virtual EmployeeRole EmployeeRole { get; set; } = null!;

    [ForeignKey("HallId")]
    [InverseProperty("Employees")]
    public virtual Hall Hall { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    [InverseProperty("Employee")]
    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
