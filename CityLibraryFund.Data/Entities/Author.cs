using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Entities;

[Table("Author")]
public partial class Author
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("Authors")]
    public virtual ICollection<Work> Works { get; set; } = new List<Work>();

    [NotMapped]
    public string FullName => LastName + " " + FirstName;
}
