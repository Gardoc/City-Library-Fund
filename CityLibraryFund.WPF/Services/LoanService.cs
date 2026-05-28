using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class LoanService
    {
        public async Task<List<LoanListDto>> GetLoansAsync(
            string? search,
            string? status,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Loans
                .AsNoTracking()
                .Include(l => l.Reader)
                .Include(l => l.LibraryItem)
                    .ThenInclude(li => li.Edition)
                        .ThenInclude(e => e.Work)
                .AsQueryable();

            if (CurrentUserService.Role == "Библиотекарь")
            {
                query = query.Where(l =>
                    l.LibraryItem.Hall.LibraryId == CurrentUserService.LibraryId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(l =>
                    (l.Reader.LastName + " " +
                     l.Reader.FirstName + " " +
                     (l.Reader.Patronymic ?? ""))
                        .Contains(search)

                    || l.LibraryItem.Edition.Work.Title.Contains(search)

                    || l.LibraryItem.InventoryNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                {
                    query = query.Where(l => l.ReturnDate == null);
                }
                else if (status == "returned")
                {
                    query = query.Where(l => l.ReturnDate != null);
                }
                else if (status == "overdue")
                {
                    query = query.Where(l =>
                        l.ReturnDate == null &&
                        l.DueDate < DateOnly.FromDateTime(DateTime.Now));
                }
            }

            query = sortBy switch
            {
                "issueDateAsc" => query.OrderBy(l => l.IssueDate),
                _ => query.OrderByDescending(l => l.IssueDate)
            };

            return await query
                .Select(l => new LoanListDto
                {
                    Id = l.Id,

                    ReaderFullName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName + " " +
                        (l.Reader.Patronymic ?? ""),

                    BookTitle = l.LibraryItem.Edition.Work.Title,
                    InventoryNumber = l.LibraryItem.InventoryNumber,

                    IssueDate = l.IssueDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,

                    Status =
                        l.ReturnDate != null
                            ? "Возвращено"
                            : l.DueDate < DateOnly.FromDateTime(DateTime.Now)
                                ? "Просрочено"
                                : "Выдано"
                })
                .ToListAsync();
        }

        public async Task<bool> ReturnLoanAsync(int loanId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var loan = await context.Loans
                .Include(l => l.LibraryItem)
                    .ThenInclude(li => li.Hall)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            if (CurrentUserService.Role == "Библиотекарь")
            {
                bool invalidLibrary =
                    loan.LibraryItem.Hall.LibraryId != CurrentUserService.LibraryId;

                if (invalidLibrary)
                {
                    return false;
                }
            }

            if (loan.ReturnDate != null)
            {
                return false;
            }

            loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now);

            int availableStatusId = await context.LibraryItemStatuses
                .Where(s => s.Name == "В наличии")
                .Select(s => s.Id)
                .FirstAsync();

            loan.LibraryItem.StatusId = availableStatusId;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ReaderIssueDto>> GetReadersForIssueAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Readers
                .Where(r => r.IsActive);

            return await query
                .OrderBy(r => r.LastName)
                .ThenBy(r => r.FirstName)
                .Select(r => new ReaderIssueDto
                {
                    Id = r.Id,

                    FullName =
                        r.LastName + " " +
                        r.FirstName + " " +
                        (r.Patronymic ?? "")
                })
                .ToListAsync();
        }

        public async Task<List<LibraryItemIssueDto>> GetAvailableLibraryItemsAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.LibraryItems
                .AsNoTracking()
                .Include(li => li.Status)
                .Include(li => li.Hall)
                .Include(li => li.Edition)
                    .ThenInclude(e => e.Work)
                .Where(li =>
                    li.Status.Name == "В наличии" &&
                    !li.OnlyReadingRoom);

            if (CurrentUserService.Role == "Библиотекарь")
            {
                query = query.Where(li =>
                    li.Hall.LibraryId == CurrentUserService.LibraryId);
            }

            return await query
                .OrderBy(li => li.Edition.Work.Title)
                .Select(li => new LibraryItemIssueDto
                {
                    Id = li.Id,

                    DisplayText =
                        li.Edition.Work.Title +
                        " | " +
                        li.InventoryNumber
                })
                .ToListAsync();
        }

        public async Task<OperationResultDto> IssueLoanAsync(IssueLoanDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var libraryItem = await context.LibraryItems
                .Include(li => li.Status)
                .Include(li => li.Hall)
                .FirstOrDefaultAsync(li => li.Id == dto.LibraryItemId);

            if (libraryItem == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Экземпляр не найден."
                };
            }

            if (CurrentUserService.Role == "Библиотекарь")
            {
                bool invalidLibrary =
                    libraryItem.Hall.LibraryId != CurrentUserService.LibraryId;

                if (invalidLibrary)
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage =
                            "Нельзя оформлять выдачу чужой библиотеки."
                    };
                }
            }

            if (libraryItem.Status.Name != "В наличии")
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Экземпляр недоступен."
                };
            }

            if (libraryItem.OnlyReadingRoom)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Экземпляр только для читального зала."
                };
            }

            Loan loan = new()
            {
                ReaderId = dto.ReaderId,
                LibraryItemId = dto.LibraryItemId,
                EmployeeId = CurrentUserService.EmployeeId,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate
            };

            context.Loans.Add(loan);

            int issuedStatusId = await context.LibraryItemStatuses
                .Where(s => s.Name == "Выдано")
                .Select(s => s.Id)
                .FirstAsync();

            libraryItem.StatusId = issuedStatusId;

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }
    }
}