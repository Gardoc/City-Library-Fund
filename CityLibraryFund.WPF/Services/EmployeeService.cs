using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class EmployeeService
    {
        public async Task<List<string>> GetEmployeeRolesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.EmployeeRoles
                .OrderBy(r => r.Name)
                .Select(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<EmployeeRole>> GetEmployeeRolesForEditAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.EmployeeRoles
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Library>> GetLibrariesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Libraries
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<Hall>> GetHallsAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Halls
                .Include(h => h.Library)
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        public async Task<List<Hall>> GetHallsByLibraryAsync(int libraryId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Halls
                .Where(h => h.LibraryId == libraryId)
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        public async Task<Hall?> GetHallAsync(int hallId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Halls
                .Include(h => h.Library)
                .FirstOrDefaultAsync(h => h.Id == hallId);
        }

        public async Task<List<EmployeeListDto>> GetEmployeesAsync(
            string? search,
            string? role,
            bool? isActive,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Employees
                .Include(e => e.EmployeeRole)
                .Include(e => e.Hall)
                    .ThenInclude(h => h.Library)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(e =>
                    (e.LastName + " " +
                     e.FirstName + " " +
                     (e.Patronymic ?? ""))
                    .Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(e => e.EmployeeRole.Name == role);
            }

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            query = sortBy switch
            {
                "fullNameDesc" =>
                    query.OrderByDescending(e => e.LastName)
                         .ThenByDescending(e => e.FirstName),

                "hireDateDesc" =>
                    query.OrderByDescending(e => e.HireDate),

                "hireDateAsc" =>
                    query.OrderBy(e => e.HireDate),

                _ =>
                    query.OrderBy(e => e.LastName)
                         .ThenBy(e => e.FirstName)
            };

            return await query
                .Select(e => new EmployeeListDto
                {
                    Id = e.Id,

                    FullName =
                        e.LastName + " " +
                        e.FirstName + " " +
                        (e.Patronymic ?? ""),

                    Role = e.EmployeeRole.Name,
                    Position = e.Position,
                    HallName = e.Hall.Name,
                    LibraryName = e.Hall.Library.Name,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EmployeeDetailsDto?> GetEmployeeDetailsAsync(int employeeId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var employee = await context.Employees
                .Include(e => e.EmployeeRole)
                .Include(e => e.Hall)
                    .ThenInclude(h => h.Library)
                .Include(e => e.Hall.HallType)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return null;
            }

            return new EmployeeDetailsDto
            {
                Id = employee.Id,

                FullName =
                    employee.LastName + " " +
                    employee.FirstName + " " +
                    (employee.Patronymic ?? ""),

                Role = employee.EmployeeRole.Name,
                Position = employee.Position,
                HallName = employee.Hall.Name,
                HallType = employee.Hall.HallType.Name,
                LibraryName = employee.Hall.Library.Name,
                Floor = employee.Hall.Floor,
                HireDate = employee.HireDate,
                Login = employee.Login,
                IsActive = employee.IsActive
            };
        }

        public async Task<EmployeeEditDto?> GetEmployeeForEditAsync(int employeeId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return null;
            }

            return new EmployeeEditDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Patronymic = employee.Patronymic,
                Position = employee.Position,
                HireDate = employee.HireDate,
                EmployeeRoleId = employee.EmployeeRoleId,
                HallId = employee.HallId,
                Login = employee.Login,
                IsActive = employee.IsActive
            };
        }

        public async Task<OperationResultDto> CreateEmployeeAsync(EmployeeEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();
            dto.Patronymic = dto.Patronymic?.Trim();
            dto.Position = dto.Position.Trim();
            dto.Login = dto.Login.Trim();
            dto.Password = dto.Password.Trim();

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите имя."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите фамилию."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Position))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите должность."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Login))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите логин."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите пароль."
                };
            }

            if (dto.Password.Length < 4)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Пароль слишком короткий."
                };
            }

            if (dto.HireDate > DateOnly.FromDateTime(DateTime.Now))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректная дата приема."
                };
            }

            bool loginExists = await context.Employees
                .AnyAsync(e => e.Login == dto.Login);

            if (loginExists)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Логин уже существует."
                };
            }

            Employee employee = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Patronymic = dto.Patronymic,
                Position = dto.Position,
                HireDate = dto.HireDate,
                EmployeeRoleId = dto.EmployeeRoleId,
                HallId = dto.HallId,
                Login = dto.Login,
                Password = dto.Password,
                IsActive = true
            };

            context.Employees.Add(employee);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> UpdateEmployeeAsync(EmployeeEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (employee == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Сотрудник не найден."
                };
            }

            if (!employee.IsActive)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Сотрудник деактивирован."
                };
            }

            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();
            dto.Patronymic = dto.Patronymic?.Trim();
            dto.Position = dto.Position.Trim();

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите имя."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите фамилию."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Position))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите должность."
                };
            }

            if (dto.HireDate > DateOnly.FromDateTime(DateTime.Now))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректная дата приема."
                };
            }

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Patronymic = dto.Patronymic;
            employee.Position = dto.Position;
            employee.HireDate = dto.HireDate;
            employee.EmployeeRoleId = dto.EmployeeRoleId;
            employee.HallId = dto.HallId;

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> ChangePasswordAsync(ChangePasswordDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);

            if (employee == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Сотрудник не найден."
                };
            }

            if (!employee.IsActive)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Сотрудник деактивирован."
                };
            }

            dto.OldPassword = dto.OldPassword.Trim();
            dto.NewPassword = dto.NewPassword.Trim();

            if (string.IsNullOrWhiteSpace(dto.OldPassword))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите текущий пароль."
                };
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите новый пароль."
                };
            }

            if (dto.NewPassword.Length < 4)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Пароль слишком короткий."
                };
            }

            if (employee.Password != dto.OldPassword)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Текущий пароль неверный."
                };
            }

            if (dto.OldPassword == dto.NewPassword)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Новый пароль должен отличаться от старого."
                };
            }

            employee.Password = dto.NewPassword;

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<bool> ToggleEmployeeActivityAsync(int employeeId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return false;
            }

            employee.IsActive = !employee.IsActive;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<EmployeeServiceStatisticsDto> GetEmployeeServiceStatisticsAsync(
            int employeeId,
            DateOnly dateFrom,
            DateOnly dateTo)
        {
            using AppDbContext context = DbContextFactory.Create();

            var loansQuery = context.Loans.Where(l =>
                l.EmployeeId == employeeId &&
                l.IssueDate >= dateFrom &&
                l.IssueDate <= dateTo);

            int readersCount = await loansQuery
                .Select(l => l.ReaderId)
                .Distinct()
                .CountAsync();

            int loansCount = await loansQuery.CountAsync();

            int returnedCount = await loansQuery
                .CountAsync(l => l.ReturnDate != null);

            int activeLoansCount = await loansQuery
                .CountAsync(l => l.ReturnDate == null);

            return new EmployeeServiceStatisticsDto
            {
                ReadersCount = readersCount,
                LoansCount = loansCount,
                ReturnedCount = returnedCount,
                ActiveLoansCount = activeLoansCount
            };
        }

        public async Task<List<EmployeeServiceHistoryDto>> GetEmployeeServiceHistoryAsync(
            int employeeId,
            DateOnly dateFrom,
            DateOnly dateTo)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Loans
                .Include(l => l.Reader)
                .Include(l => l.LibraryItem)
                    .ThenInclude(li => li.Edition)
                    .ThenInclude(e => e.Work)
                .Where(l =>
                    l.EmployeeId == employeeId &&
                    l.IssueDate >= dateFrom &&
                    l.IssueDate <= dateTo)
                .OrderByDescending(l => l.IssueDate)
                .Select(l => new EmployeeServiceHistoryDto
                {
                    ReaderName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName + " " +
                        (l.Reader.Patronymic ?? ""),

                    WorkTitle = l.LibraryItem.Edition.Work.Title,
                    LoanDate = l.IssueDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    IsReturned = l.ReturnDate != null
                })
                .ToListAsync();
        }
    }
}