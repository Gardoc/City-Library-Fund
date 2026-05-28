using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.Data.Entities;

[Keyless]
public partial class VwDashboardStatistic
{
    public int? ReadersCount { get; set; }

    public int? LibraryItemsCount { get; set; }

    public int? ActiveLoansCount { get; set; }

    public int? OverdueReadersCount { get; set; }
}
