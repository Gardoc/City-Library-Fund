using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Services
{
    public class AuthorService
    {
        public async Task<List<AuthorListDto>> GetAuthorsAsync(
            string? search,
            string? country,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Authors
                .Include(a => a.Works)
                .AsQueryable();
     
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(a =>
                    (a.LastName + " " + a.FirstName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(a => a.Country == country);
            }

            query = sortBy switch
            {
                "fullNameDesc" => query
                    .OrderByDescending(a => a.LastName)
                    .ThenByDescending(a => a.FirstName),

                "birthDateDesc" => query
                    .OrderByDescending(a => a.BirthDate),

                "birthDateAsc" => query
                    .OrderBy(a => a.BirthDate),

                "worksCountDesc" => query
                    .OrderByDescending(a => a.Works.Count),

                _ => query
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
            };

            return await query
                .Select(a => new AuthorListDto
                {
                    Id = a.Id,

                    FullName =
                        a.LastName + " " +
                        a.FirstName,

                    BirthDate = a.BirthDate,
                    Country = a.Country ?? "",
                    WorksCount = a.Works.Count
                })
                .ToListAsync();
        }

        public async Task<List<string>> GetCountriesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Authors
                .Where(a => a.Country != null && a.Country != "")
                .Select(a => a.Country!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<AuthorDetailsDto?> GetAuthorDetailsAsync(int authorId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var author = await context.Authors
                .Include(a => a.Works)
                .ThenInclude(w => w.Editions)
                .ThenInclude(e => e.LibraryItems)
                .FirstOrDefaultAsync(a => a.Id == authorId);

            if (author == null)
            {
                return null;
            }

            int copiesCount = author.Works
                .SelectMany(w => w.Editions)
                .SelectMany(e => e.LibraryItems)
                .Count();

            string? latestWork = author.Works
                .OrderByDescending(w => w.YearWritten)
                .Select(w => w.Title)
                .FirstOrDefault();

            return new AuthorDetailsDto
            {
                Id = author.Id,

                FullName =
                    author.LastName + " " +
                    author.FirstName,

                BirthDate = author.BirthDate,
                Country = author.Country ?? "",
                WorksCount = author.Works.Count,
                CopiesCount = copiesCount,
                LatestWork = latestWork
            };
        }

        public async Task<AuthorEditDto?> GetAuthorForEditAsync(int authorId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var author = await context.Authors
                .FirstOrDefaultAsync(a => a.Id == authorId);

            if (author == null)
            {
                return null;
            }

            return new AuthorEditDto
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                BirthDate = author.BirthDate,
                Country = author.Country
            };
        }

        public async Task<OperationResultDto> CreateAuthorAsync(AuthorEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();
            dto.Country = dto.Country?.Trim();

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

            if (dto.BirthDate > DateOnly.FromDateTime(DateTime.Now))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректная дата рождения."
                };
            }

            bool exists = await context.Authors.AnyAsync(a =>
                a.FirstName == dto.FirstName &&
                a.LastName == dto.LastName &&
                a.BirthDate == dto.BirthDate);

            if (exists)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Такой автор уже существует."
                };
            }

            Author author = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Country = dto.Country
            };

            context.Authors.Add(author);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> UpdateAuthorAsync(AuthorEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var author = await context.Authors
                .FirstOrDefaultAsync(a => a.Id == dto.Id);

            if (author == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Автор не найден."
                };
            }

            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();
            dto.Country = dto.Country?.Trim();

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

            if (dto.BirthDate > DateOnly.FromDateTime(DateTime.Now))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректная дата рождения."
                };
            }

            author.FirstName = dto.FirstName;
            author.LastName = dto.LastName;
            author.BirthDate = dto.BirthDate;
            author.Country = dto.Country;

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> DeleteAuthorAsync(int authorId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var author = await context.Authors
                .Include(a => a.Works)
                .FirstOrDefaultAsync(a => a.Id == authorId);

            if (author == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Автор не найден."
                };
            }

            bool hasWorks = author.Works.Any();

            if (hasWorks)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Нельзя удалить автора, связанного с произведениями."
                };
            }

            context.Authors.Remove(author);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<List<AuthorInventoryDto>> GetAuthorInventoryAsync(int authorId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.LibraryItems
                .Include(li => li.Edition)
                .ThenInclude(e => e.Work)
                .ThenInclude(w => w.Authors)

                .Include(li => li.Hall)
                .ThenInclude(h => h.Library)

                .Where(li =>
                    li.Edition.Work.Authors.Any(a => a.Id == authorId))

                .OrderBy(li => li.InventoryNumber)

                .Select(li => new AuthorInventoryDto
                {
                    InventoryNumber = li.InventoryNumber,
                    WorkTitle = li.Edition.Work.Title,
                    LibraryName = li.Hall.Library.Name
                })
                .ToListAsync();
        }
    }
}