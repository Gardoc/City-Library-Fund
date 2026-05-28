using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class HallService
    {
        public async Task<List<string>> GetLibrariesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Libraries
                .OrderBy(l => l.Name)
                .Select(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<HallListDto>> GetHallsAsync(
            string? search,
            string? library,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Halls
                .Include(h => h.Library)
                .Include(h => h.HallType)
                .Include(h => h.Employees)
                .Include(h => h.LibraryItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(h => h.Name.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(library))
            {
                query = query.Where(h => h.Library.Name == library);
            }

            query = sortBy switch
            {
                "nameDesc" =>
                    query.OrderByDescending(h => h.Name),

                "floorAsc" =>
                    query.OrderBy(h => h.Floor),

                "floorDesc" =>
                    query.OrderByDescending(h => h.Floor),

                "itemsCountDesc" =>
                    query.OrderByDescending(h => h.LibraryItems.Count),

                _ =>
                    query.OrderBy(h => h.Name)
            };

            return await query
                .Select(h => new HallListDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    LibraryName = h.Library.Name,
                    HallType = h.HallType.Name,
                    Floor = h.Floor,

                    EmployeesCount =
                        h.Employees.Count(e => e.IsActive),

                    LibraryItemsCount =
                        h.LibraryItems.Count()
                })
                .ToListAsync();
        }

        public async Task<HallDetailsDto?> GetHallDetailsAsync(int hallId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var hall = await context.Halls
                .Include(h => h.Library)
                .Include(h => h.HallType)
                .Include(h => h.Employees)
                .Include(h => h.LibraryItems)
                    .ThenInclude(li => li.Loans)
                .Include(h => h.Visits)
                .FirstOrDefaultAsync(h => h.Id == hallId);

            if (hall == null)
            {
                return null;
            }

            int activeLoansCount = hall.LibraryItems
                .SelectMany(li => li.Loans)
                .Count(l => l.ReturnDate == null);

            return new HallDetailsDto
            {
                Id = hall.Id,
                Name = hall.Name,
                LibraryName = hall.Library.Name,
                HallType = hall.HallType.Name,
                Floor = hall.Floor,

                EmployeesCount =
                    hall.Employees.Count(e => e.IsActive),

                LibraryItemsCount =
                    hall.LibraryItems.Count,

                ActiveLoansCount =
                    activeLoansCount,

                VisitsCount =
                    hall.Visits.Count
            };
        }

        public async Task<List<HallEmployeeDto>> GetHallEmployeesAsync(int hallId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Employees
                .Include(e => e.EmployeeRole)
                .Where(e =>
                    e.HallId == hallId &&
                    e.IsActive &&
                    e.EmployeeRole.Name == "Библиотекарь")
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new HallEmployeeDto
                {
                    FullName =
                        e.LastName + " " +
                        e.FirstName + " " +
                        (e.Patronymic ?? ""),

                    Position = e.Position,
                    HireDate = e.HireDate
                })
                .ToListAsync();
        }
    }
}