using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class WorkService
    {
        public async Task<List<WorkListDto>> GetWorksAsync(
            string? search,
            int? workTypeId,
            string? language,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Works
                .Include(w => w.WorkType)
                .Include(w => w.Authors)
                .Include(w => w.Editions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(w =>
                    w.Title.Contains(search) ||
                    w.Authors.Any(a =>
                        (a.LastName + " " + a.FirstName).Contains(search)));
            }

            if (workTypeId.HasValue)
            {
                query = query.Where(w => w.WorkTypeId == workTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                query = query.Where(w => w.Language == language);
            }

            query = sortBy switch
            {
                "titleDesc" => query.OrderByDescending(w => w.Title),

                "yearAsc" => query.OrderBy(w => w.YearWritten),

                "yearDesc" => query.OrderByDescending(w => w.YearWritten),

                "editionsCountDesc" => query.OrderByDescending(w => w.Editions.Count),

                _ => query.OrderBy(w => w.Title)
            };

            return await query
                .Select(w => new WorkListDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    WorkType = w.WorkType.Name,

                    Authors = string.Join(
                        ", ",
                        w.Authors
                            .OrderBy(a => a.LastName)
                            .Select(a => a.LastName + " " + a.FirstName)),

                    YearWritten = w.YearWritten,
                    Language = w.Language ?? "",
                    EditionsCount = w.Editions.Count
                })
                .ToListAsync();
        }

        public async Task<List<WorkType>> GetWorkTypesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.WorkTypes
                .OrderBy(wt => wt.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetLanguagesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Works
                .Where(w => w.Language != null && w.Language != "")
                .Select(w => w.Language!)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();
        }

        public async Task<List<Author>> GetAuthorsAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Authors
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();
        }

        public async Task<WorkDetailsDto?> GetWorkDetailsAsync(int workId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var work = await context.Works
                .Include(w => w.WorkType)
                .Include(w => w.Authors)
                .Include(w => w.Editions)
                    .ThenInclude(e => e.LibraryItems)
                .FirstOrDefaultAsync(w => w.Id == workId);

            if (work == null)
            {
                return null;
            }

            int copiesCount = work.Editions
                .SelectMany(e => e.LibraryItems)
                .Count();

            string? latestEdition = work.Editions
                .OrderByDescending(e => e.PublishYear)
                .Select(e =>
                    e.Publisher + ", " +
                    e.PublishYear +
                    (e.EditionNumber != null
                        ? ", изд. №" + e.EditionNumber
                        : ""))
                .FirstOrDefault();

            return new WorkDetailsDto
            {
                Id = work.Id,
                Title = work.Title,
                WorkType = work.WorkType.Name,

                Authors = string.Join(
                    ", ",
                    work.Authors
                        .OrderBy(a => a.LastName)
                        .Select(a => a.LastName + " " + a.FirstName)),

                YearWritten = work.YearWritten,
                Language = work.Language ?? "",
                Description = work.Description,
                EditionsCount = work.Editions.Count,
                CopiesCount = copiesCount,
                LatestEdition = latestEdition
            };
        }

        public async Task<WorkEditDto?> GetWorkForEditAsync(int workId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var work = await context.Works
                .Include(w => w.Authors)
                .FirstOrDefaultAsync(w => w.Id == workId);

            if (work == null)
            {
                return null;
            }

            return new WorkEditDto
            {
                Id = work.Id,
                WorkTypeId = work.WorkTypeId,
                Title = work.Title,
                YearWritten = work.YearWritten,
                Description = work.Description,
                Language = work.Language,

                AuthorIds = work.Authors
                    .Select(a => a.Id)
                    .ToList()
            };
        }

        public async Task<OperationResultDto> CreateWorkAsync(WorkEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            dto.Title = dto.Title.Trim();
            dto.Language = dto.Language?.Trim();
            dto.Description = dto.Description?.Trim();

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите название произведения."
                };
            }

            if (!dto.AuthorIds.Any())
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Выберите хотя бы одного автора."
                };
            }

            if (dto.YearWritten > DateTime.Now.Year)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный год написания."
                };
            }

            bool exists = await context.Works.AnyAsync(w =>
                w.Title == dto.Title &&
                w.YearWritten == dto.YearWritten);

            if (exists)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Такое произведение уже существует."
                };
            }

            List<Author> authors = await context.Authors
                .Where(a => dto.AuthorIds.Contains(a.Id))
                .ToListAsync();

            Work work = new()
            {
                Title = dto.Title,
                WorkTypeId = dto.WorkTypeId,
                YearWritten = dto.YearWritten,
                Description = dto.Description,
                Language = dto.Language,
                Authors = authors
            };

            context.Works.Add(work);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> UpdateWorkAsync(WorkEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var work = await context.Works
                .Include(w => w.Authors)
                .FirstOrDefaultAsync(w => w.Id == dto.Id);

            if (work == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Произведение не найдено."
                };
            }

            dto.Title = dto.Title.Trim();
            dto.Language = dto.Language?.Trim();
            dto.Description = dto.Description?.Trim();

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите название произведения."
                };
            }

            if (!dto.AuthorIds.Any())
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Выберите хотя бы одного автора."
                };
            }

            if (dto.YearWritten > DateTime.Now.Year)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный год написания."
                };
            }

            List<Author> authors = await context.Authors
                .Where(a => dto.AuthorIds.Contains(a.Id))
                .ToListAsync();

            work.Title = dto.Title;
            work.WorkTypeId = dto.WorkTypeId;
            work.YearWritten = dto.YearWritten;
            work.Description = dto.Description;
            work.Language = dto.Language;

            work.Authors.Clear();

            foreach (Author author in authors)
            {
                work.Authors.Add(author);
            }

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> DeleteWorkAsync(int workId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var work = await context.Works
                .Include(w => w.Editions)
                .FirstOrDefaultAsync(w => w.Id == workId);

            if (work == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Произведение не найдено."
                };
            }

            bool hasEditions = work.Editions.Any();

            if (hasEditions)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Нельзя удалить произведение, у которого есть издания."
                };
            }

            context.Works.Remove(work);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<List<WorkInventoryDto>> GetWorkInventoryAsync(int workId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.LibraryItems
                .Include(li => li.Edition)
                    .ThenInclude(e => e.Work)
                .Include(li => li.Hall)
                    .ThenInclude(h => h.Library)
                .Where(li => li.Edition.WorkId == workId)
                .OrderBy(li => li.InventoryNumber)
                .Select(li => new WorkInventoryDto
                {
                    InventoryNumber = li.InventoryNumber,

                    EditionInfo =
                        li.Edition.Work.Title + " (" +
                        li.Edition.Publisher + ", " +
                        li.Edition.PublishYear + ")",

                    LibraryName = li.Hall.Library.Name
                })
                .ToListAsync();
        }

        public async Task<List<WorkReaderDto>> GetWorkReadersAsync(int workId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Loans
                .Include(l => l.Reader)
                    .ThenInclude(r => r.Library)
                .Include(l => l.LibraryItem)
                    .ThenInclude(li => li.Edition)
                .Where(l =>
                    l.LibraryItem.Edition.WorkId == workId &&
                    l.ReturnDate == null)
                .OrderBy(l => l.Reader.LastName)
                .ThenBy(l => l.Reader.FirstName)
                .Select(l => new WorkReaderDto
                {
                    ReaderFullName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName + " " +
                        (l.Reader.Patronymic ?? ""),

                    Phone = l.Reader.Phone,
                    LibraryName = l.Reader.Library.Name,
                    IssueDate = l.IssueDate,
                    DueDate = l.DueDate
                })
                .ToListAsync();
        }
    }
}