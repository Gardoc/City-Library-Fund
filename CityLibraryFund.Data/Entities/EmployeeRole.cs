using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.Entities
{
    [Table("EmployeeRole")]
    [Index("Name", Name = "UQ__Employee__737584F6BFF9EF79", IsUnique = true)]
    public partial class EmployeeRole
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty("EmployeeRole")]
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
