using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResultDto> LoginAsync(string login, string password)
        {
            login = login.Trim();

            if (string.IsNullOrWhiteSpace(login))
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите логин."
                };
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите пароль."
                };
            }

            var employee = await _context.Employees
                .Include(e => e.EmployeeRole)
                .Include(e => e.Hall)
                .ThenInclude(h => h.Library)
                .FirstOrDefaultAsync(e =>
                    e.Login == login &&
                    e.Password == password);

            if (employee == null)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Неверный логин или пароль."
                };
            }

            if (!employee.IsActive)
            {
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Пользователь деактивирован."
                };
            }

            return new AuthResultDto
            {
                IsSuccess = true,
                EmployeeId = employee.Id,

                FullName =
                    $"{employee.LastName} " +
                    $"{employee.FirstName} " +
                    $"{employee.Patronymic}",

                Role = employee.EmployeeRole.Name,
                HallId = employee.HallId,
                HallName = employee.Hall?.Name ?? string.Empty,
                LibraryId = employee.Hall?.LibraryId ?? 0,

                LibraryName =
                    employee.Hall?.Library?.Name ??
                    string.Empty
            };
        }
    }
}