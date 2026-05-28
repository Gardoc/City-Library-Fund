using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class EditionService
    {
        public async Task<List<EditionListDto>> GetEditionsAsync(
            string? search,
            int? workTypeId,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Editions
                .Include(e => e.Work)
                    .ThenInclude(w => w.WorkType)
                .Include(e => e.Work)
                    .ThenInclude(w => w.Authors)
                .Include(e => e.LibraryItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(e =>
                    e.Work.Title.Contains(search) ||
                    e.Publisher.Contains(search) ||
                    (e.Isbn != null && e.Isbn.Contains(search)));
            }

            if (workTypeId.HasValue)
            {
                query = query.Where(e =>
                    e.Work.WorkTypeId == workTypeId.Value);
            }

            query = sortBy switch
            {
                "titleDesc" =>
                    query.OrderByDescending(e => e.Work.Title),

                "publishYearAsc" =>
                    query.OrderBy(e => e.PublishYear),

                "publishYearDesc" =>
                    query.OrderByDescending(e => e.PublishYear),

                "copiesCountDesc" =>
                    query.OrderByDescending(e => e.LibraryItems.Count),

                _ =>
                    query.OrderBy(e => e.Work.Title)
            };

            return await query
                .Select(e => new EditionListDto
                {
                    Id = e.Id,
                    WorkTitle = e.Work.Title,

                    Authors = string.Join(
                        ", ",
                        e.Work.Authors
                            .OrderBy(a => a.LastName)
                            .Select(a =>
                                a.LastName + " " + a.FirstName)),

                    Publisher = e.Publisher,
                    PublishYear = e.PublishYear,
                    Isbn = e.Isbn ?? "",
                    EditionNumber = e.EditionNumber,
                    CopiesCount = e.LibraryItems.Count
                })
                .ToListAsync();
        }

        public async Task<List<Work>> GetWorksAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Works
                .Include(w => w.Authors)
                .OrderBy(w => w.Title)
                .ToListAsync();
        }

        public async Task<List<WorkType>> GetWorkTypesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.WorkTypes
                .OrderBy(wt => wt.Name)
                .ToListAsync();
        }

        public async Task<EditionDetailsDto?> GetEditionDetailsAsync(int editionId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var edition = await context.Editions
                .Include(e => e.Work)
                    .ThenInclude(w => w.WorkType)
                .Include(e => e.Work)
                    .ThenInclude(w => w.Authors)
                .Include(e => e.LibraryItems)
                .FirstOrDefaultAsync(e => e.Id == editionId);

            if (edition == null)
            {
                return null;
            }

            return new EditionDetailsDto
            {
                Id = edition.Id,
                WorkTitle = edition.Work.Title,

                Authors = string.Join(
                    ", ",
                    edition.Work.Authors
                        .OrderBy(a => a.LastName)
                        .Select(a =>
                            a.LastName + " " + a.FirstName)),

                WorkType = edition.Work.WorkType.Name,
                Publisher = edition.Publisher,
                PublishYear = edition.PublishYear,
                Isbn = edition.Isbn,
                PageCount = edition.PageCount,
                EditionNumber = edition.EditionNumber,
                CopiesCount = edition.LibraryItems.Count,
                Language = edition.Work.Language ?? "",
                Description = edition.Work.Description
            };
        }

        public async Task<EditionEditDto?> GetEditionForEditAsync(int editionId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var edition = await context.Editions
                .FirstOrDefaultAsync(e => e.Id == editionId);

            if (edition == null)
            {
                return null;
            }

            return new EditionEditDto
            {
                Id = edition.Id,
                WorkId = edition.WorkId,
                Publisher = edition.Publisher,
                PublishYear = edition.PublishYear,
                Isbn = edition.Isbn,
                PageCount = edition.PageCount,
                EditionNumber = edition.EditionNumber
            };
        }

        public async Task<OperationResultDto> CreateEditionAsync(EditionEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            dto.Publisher = dto.Publisher.Trim();
            dto.Isbn = dto.Isbn?.Trim();

            if (string.IsNullOrWhiteSpace(dto.Publisher))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите издательство."
                };
            }

            if (dto.PublishYear > DateTime.Now.Year)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный год издания."
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.Isbn))
            {
                bool isbnExists = await context.Editions
                    .AnyAsync(e => e.Isbn == dto.Isbn);

                if (isbnExists)
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "ISBN уже существует."
                    };
                }
            }

            Edition edition = new()
            {
                WorkId = dto.WorkId,
                Publisher = dto.Publisher,
                PublishYear = dto.PublishYear,
                Isbn = dto.Isbn,
                PageCount = dto.PageCount,
                EditionNumber = dto.EditionNumber
            };

            context.Editions.Add(edition);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> UpdateEditionAsync(EditionEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var edition = await context.Editions
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (edition == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Издание не найдено."
                };
            }

            dto.Publisher = dto.Publisher.Trim();
            dto.Isbn = dto.Isbn?.Trim();

            if (string.IsNullOrWhiteSpace(dto.Publisher))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите издательство."
                };
            }

            if (dto.PublishYear > DateTime.Now.Year)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный год издания."
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.Isbn))
            {
                bool isbnExists = await context.Editions
                    .AnyAsync(e =>
                        e.Isbn == dto.Isbn &&
                        e.Id != dto.Id);

                if (isbnExists)
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "ISBN уже существует."
                    };
                }
            }

            edition.WorkId = dto.WorkId;
            edition.Publisher = dto.Publisher;
            edition.PublishYear = dto.PublishYear;
            edition.Isbn = dto.Isbn;
            edition.PageCount = dto.PageCount;
            edition.EditionNumber = dto.EditionNumber;

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> DeleteEditionAsync(int editionId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var edition = await context.Editions
                .Include(e => e.LibraryItems)
                .FirstOrDefaultAsync(e => e.Id == editionId);

            if (edition == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Издание не найдено."
                };
            }

            bool hasCopies = edition.LibraryItems.Any();

            if (hasCopies)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Нельзя удалить издание, у которого есть экземпляры."
                };
            }

            context.Editions.Remove(edition);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<List<EditionReaderDto>> GetEditionReadersAsync(int editionId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var readers = await context.Loans
                .Include(l => l.Reader)
                    .ThenInclude(r => r.ReaderType)
                .Include(l => l.LibraryItem)
                    .ThenInclude(li => li.Edition)
                .Where(l =>
                    l.LibraryItem.EditionId == editionId &&
                    l.ReturnDate == null)
                .OrderBy(l => l.DueDate)
                .Select(l => new EditionReaderDto
                {
                    ReaderName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName + " " +
                        (l.Reader.Patronymic ?? ""),

                    ReaderType = l.Reader.ReaderType.Name,
                    InventoryNumber = l.LibraryItem.InventoryNumber,
                    IssueDate = l.IssueDate,
                    DueDate = l.DueDate
                })
                .ToListAsync();

            foreach (var reader in readers)
            {
                reader.DaysLeft =
                    reader.DueDate.DayNumber -
                    DateOnly.FromDateTime(DateTime.Now).DayNumber;
            }

            return readers;
        }
    }
}