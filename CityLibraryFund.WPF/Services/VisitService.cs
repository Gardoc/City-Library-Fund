using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class VisitService
    {
        public async Task<List<VisitListDto>> GetVisitsAsync(
            string? search,
            int? hallId,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Visits
                .AsNoTracking()
                .Include(v => v.Reader)
                .Include(v => v.Employee)
                .Include(v => v.Hall)
                    .ThenInclude(h => h.Library)
                .AsQueryable();

            if (CurrentUserService.Role == "Библиотекарь")
            {
                query = query.Where(v =>
                    v.Hall.LibraryId == CurrentUserService.LibraryId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(v =>
                    (v.Reader.LastName + " " +
                     v.Reader.FirstName + " " +
                     (v.Reader.Patronymic ?? ""))
                    .Contains(search)

                    || v.Hall.Name.Contains(search)

                    || v.Hall.Library.Name.Contains(search));
            }

            if (hallId.HasValue)
            {
                query = query.Where(v => v.HallId == hallId.Value);
            }

            query = sortBy switch
            {
                "visitDateAsc" => query.OrderBy(v => v.VisitDate),

                _ => query.OrderByDescending(v => v.VisitDate)
            };

            return await query
                .Select(v => new VisitListDto
                {
                    Id = v.Id,

                    ReaderFullName =
                        v.Reader.LastName + " " +
                        v.Reader.FirstName + " " +
                        (v.Reader.Patronymic ?? ""),

                    HallName = v.Hall.Name,

                    LibraryName = v.Hall.Library.Name,

                    EmployeeFullName =
                        v.Employee.LastName + " " +
                        v.Employee.FirstName,

                    VisitDate = v.VisitDate
                })
                .ToListAsync();
        }

        public async Task<List<ReaderVisitDto>> GetReadersForVisitAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Readers
                .Where(r => r.IsActive);

            return await query
                .OrderBy(r => r.LastName)
                .ThenBy(r => r.FirstName)
                .Select(r => new ReaderVisitDto
                {
                    Id = r.Id,

                    FullName =
                        r.LastName + " " +
                        r.FirstName + " " +
                        (r.Patronymic ?? "")
                })
                .ToListAsync();
        }

        public async Task<List<HallVisitDto>> GetHallsForVisitAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Halls
                .Include(h => h.Library)
                .AsQueryable();

            if (CurrentUserService.Role == "Библиотекарь")
            {
                query = query.Where(h =>
                    h.LibraryId == CurrentUserService.LibraryId);
            }

            return await query
                .OrderBy(h => h.Library.Name)
                .ThenBy(h => h.Name)
                .Select(h => new HallVisitDto
                {
                    Id = h.Id,

                    DisplayName =
                        h.Library.Name +
                        " | " +
                        h.Name
                })
                .ToListAsync();
        }

        public async Task<OperationResultDto> CreateVisitAsync(CreateVisitDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var reader = await context.Readers
                .FirstOrDefaultAsync(r => r.Id == dto.ReaderId);

            if (reader == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Читатель не найден."
                };
            }

            if (!reader.IsActive)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Читатель деактивирован."
                };
            }

            var hall = await context.Halls
                .Include(h => h.Library)
                .FirstOrDefaultAsync(h => h.Id == dto.HallId);

            if (hall == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Зал не найден."
                };
            }

            if (CurrentUserService.Role == "Библиотекарь")
            {
                bool invalidHall =
                    hall.LibraryId != CurrentUserService.LibraryId;

                if (invalidHall)
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage =
                            "Нельзя оформлять посещения другой библиотеки."
                    };
                }
            }

            Visit visit = new()
            {
                ReaderId = dto.ReaderId,
                EmployeeId = CurrentUserService.EmployeeId,
                HallId = dto.HallId,
                VisitDate = dto.VisitDate
            };

            context.Visits.Add(visit);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }
    }
}