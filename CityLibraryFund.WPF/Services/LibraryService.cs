using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class LibraryService
    {
        public async Task<List<LibraryListDto>> GetLibrariesAsync(
            string? search,
            bool? isActive,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Libraries
                .Include(l => l.Halls)
                    .ThenInclude(h => h.LibraryItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(l =>
                    l.Name.Contains(search) ||
                    l.Address.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(l => l.IsActive == isActive.Value);
            }

            query = sortBy switch
            {
                "nameDesc" =>
                    query.OrderByDescending(l => l.Name),

                "itemsCountDesc" =>
                    query.OrderByDescending(l =>
                        l.Halls.SelectMany(h => h.LibraryItems).Count()),

                "hallsCountDesc" =>
                    query.OrderByDescending(l => l.Halls.Count),

                _ =>
                    query.OrderBy(l => l.Name)
            };

            return await query
                .Select(l => new LibraryListDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    Phone = l.Phone,
                    HallsCount = l.Halls.Count,
                    LibraryItemsCount =
                        l.Halls.SelectMany(h => h.LibraryItems).Count()
                })
                .ToListAsync();
        }

        public async Task<LibraryDetailsDto?> GetLibraryDetailsAsync(int libraryId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var library = await context.Libraries
                .Include(l => l.Halls)
                    .ThenInclude(h => h.Employees)
                .Include(l => l.Halls)
                    .ThenInclude(h => h.LibraryItems)
                        .ThenInclude(li => li.Loans)
                .Include(l => l.Readers)
                .FirstOrDefaultAsync(l => l.Id == libraryId);

            if (library == null)
            {
                return null;
            }

            int employeesCount =
                library.Halls.SelectMany(h => h.Employees).Count();

            int libraryItemsCount =
                library.Halls.SelectMany(h => h.LibraryItems).Count();

            int activeLoansCount =
                library.Halls
                    .SelectMany(h => h.LibraryItems)
                    .SelectMany(li => li.Loans)
                    .Count(l => l.ReturnDate == null);

            return new LibraryDetailsDto
            {
                Id = library.Id,
                Name = library.Name,
                Address = library.Address,
                Phone = library.Phone,
                IsActive = library.IsActive,
                HallsCount = library.Halls.Count,
                EmployeesCount = employeesCount,
                ReadersCount = library.Readers.Count,
                LibraryItemsCount = libraryItemsCount,
                ActiveLoansCount = activeLoansCount
            };
        }
    }
}