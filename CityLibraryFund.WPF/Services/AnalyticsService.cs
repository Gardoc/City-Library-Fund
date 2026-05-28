using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class AnalyticsService
    {
        public async Task<List<ReaderAnalyticsDto>> GetReadersByCharacteristicsAsync(
            string? readerType,
            string? university,
            string? faculty,
            string? scientificTopic)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Readers
                .Include(r => r.ReaderType)
                .Include(r => r.Library)
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.Scientist)
                .Include(r => r.Schooler)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(readerType))
            {
                query = query.Where(r => r.ReaderType.Name == readerType);
            }

            if (!string.IsNullOrWhiteSpace(university))
            {
                query = query.Where(r =>

                    (r.Student != null &&
                     r.Student.University.Contains(university))

                    ||

                    (r.Teacher != null &&
                     r.Teacher.University.Contains(university)));
            }

            if (!string.IsNullOrWhiteSpace(faculty))
            {
                query = query.Where(r =>
                    r.Student != null &&
                    r.Student.Faculty.Contains(faculty));
            }

            if (!string.IsNullOrWhiteSpace(scientificTopic))
            {
                query = query.Where(r =>
                    r.Scientist != null &&
                    r.Scientist.ScientificTopic.Contains(scientificTopic));
            }

            return await query
                .OrderBy(r => r.LastName)
                .ThenBy(r => r.FirstName)
                .Select(r => new ReaderAnalyticsDto
                {
                    FullName =
                        r.LastName + " " +
                        r.FirstName + " " +
                        (r.Patronymic ?? ""),

                    ReaderType = r.ReaderType.Name,
                    LibraryName = r.Library.Name,
                    Phone = r.Phone,

                    AdditionalInfo =

                        r.Student != null
                        ? r.Student.University

                        : r.Teacher != null
                        ? r.Teacher.University

                        : r.Scientist != null
                        ? r.Scientist.Organization

                        : r.Schooler != null
                        ? r.Schooler.School

                        : null
                })
                .ToListAsync();
        }

        public async Task<List<ReaderAnalyticsDto>> GetOverdueReadersAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Loans
                .Include(l => l.Reader)
                .ThenInclude(r => r.ReaderType)
                .Include(l => l.Reader.Library)

                .Where(l =>
                    l.ReturnDate == null &&
                    l.DueDate < DateOnly.FromDateTime(DateTime.Now))

                .OrderBy(l => l.DueDate)

                .Select(l => new ReaderAnalyticsDto
                {
                    FullName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName + " " +
                        (l.Reader.Patronymic ?? ""),

                    ReaderType = l.Reader.ReaderType.Name,
                    LibraryName = l.Reader.Library.Name,
                    Phone = l.Reader.Phone,

                    AdditionalInfo =
                        "Просрочка до " +
                        l.DueDate.ToString("dd.MM.yyyy")
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<ReaderAnalyticsDto>> GetInactiveReadersAsync(
            DateTime inactiveSince)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Readers
                .Include(r => r.ReaderType)
                .Include(r => r.Library)

                .Where(r =>
                    !context.Visits.Any(v =>
                        v.ReaderId == r.Id &&
                        v.VisitDate >= inactiveSince))

                .OrderBy(r => r.LastName)

                .Select(r => new ReaderAnalyticsDto
                {
                    FullName =
                        r.LastName + " " +
                        r.FirstName + " " +
                        (r.Patronymic ?? ""),

                    ReaderType = r.ReaderType.Name,
                    LibraryName = r.Library.Name,
                    Phone = r.Phone,

                    AdditionalInfo = "Нет посещений"
                })
                .ToListAsync();
        }

        public async Task<List<LoanAnalyticsDto>> GetReadersByWorkAsync(
            string workTitle,
            DateOnly dateFrom,
            DateOnly dateTo)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Loans
                .Include(l => l.Reader)

                .Include(l => l.LibraryItem)
                .ThenInclude(li => li.Hall)
                .ThenInclude(h => h.Library)

                .Include(l => l.LibraryItem)
                .ThenInclude(li => li.Edition)
                .ThenInclude(e => e.Work)

                .Where(l =>
                    l.IssueDate >= dateFrom &&
                    l.IssueDate <= dateTo &&
                    l.LibraryItem.Edition.Work.Title.Contains(workTitle))

                .OrderByDescending(l => l.IssueDate)

                .Select(l => new LoanAnalyticsDto
                {
                    ReaderFullName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName,

                    WorkTitle = l.LibraryItem.Edition.Work.Title,
                    EditionTitle = l.LibraryItem.Edition.Publisher,
                    LibraryName = l.LibraryItem.Hall.Library.Name,
                    IssueDate = l.IssueDate,
                    ReturnDate = l.ReturnDate
                })
                .ToListAsync();
        }

        public async Task<List<LoanAnalyticsDto>> GetForeignLibraryLoansAsync(
            int readerId,
            DateOnly dateFrom,
            DateOnly dateTo)
        {
            using AppDbContext context = DbContextFactory.Create();

            int readerLibraryId = await context.Readers
                .Where(r => r.Id == readerId)
                .Select(r => r.LibraryId)
                .FirstOrDefaultAsync();

            return await context.Loans
                .Include(l => l.Reader)

                .Include(l => l.LibraryItem)
                .ThenInclude(li => li.Hall)
                .ThenInclude(h => h.Library)

                .Include(l => l.LibraryItem)
                .ThenInclude(li => li.Edition)
                .ThenInclude(e => e.Work)

                .Where(l =>
                    l.ReaderId == readerId &&
                    l.IssueDate >= dateFrom &&
                    l.IssueDate <= dateTo &&
                    l.LibraryItem.Hall.LibraryId != readerLibraryId)

                .OrderByDescending(l => l.IssueDate)

                .Select(l => new LoanAnalyticsDto
                {
                    ReaderFullName =
                        l.Reader.LastName + " " +
                        l.Reader.FirstName,

                    WorkTitle = l.LibraryItem.Edition.Work.Title,
                    EditionTitle = l.LibraryItem.Edition.Publisher,
                    LibraryName = l.LibraryItem.Hall.Library.Name,
                    IssueDate = l.IssueDate,
                    ReturnDate = l.ReturnDate
                })
                .ToListAsync();
        }

        public async Task<List<FundAnalyticsDto>> GetShelfLoansAsync(
            int libraryId,
            string shelfCode)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.LibraryItems
                .Include(li => li.Hall)
                .ThenInclude(h => h.Library)

                .Include(li => li.Edition)
                .ThenInclude(e => e.Work)

                .Include(li => li.Status)

                .Where(li =>
                    li.Hall.LibraryId == libraryId &&
                    li.InventoryNumber == shelfCode &&
                    li.Status.Name == "Выдано")

                .OrderBy(li => li.InventoryNumber)

                .Select(li => new FundAnalyticsDto
                {
                    InventoryNumber = li.InventoryNumber,
                    WorkTitle = li.Edition.Work.Title,
                    LibraryName = li.Hall.Library.Name,
                    HallName = li.Hall.Name,
                    ShelfCode = li.InventoryNumber,
                    Status = li.Status.Name
                })
                .ToListAsync();
        }

        public async Task<List<FundAnalyticsDto>> GetReceiptWriteOffAsync(
            DateOnly dateFrom,
            DateOnly dateTo,
            bool isReceipt)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.LibraryItems
                .Include(li => li.Hall)
                .ThenInclude(h => h.Library)

                .Include(li => li.Edition)
                .ThenInclude(e => e.Work)

                .Include(li => li.Status)
                .AsQueryable();

            if (isReceipt)
            {
                query = query.Where(li =>
                    li.ArrivalDate >= dateFrom &&
                    li.ArrivalDate <= dateTo);
            }
            else
            {
                query = query.Where(li =>
                    li.WriteOffDate != null &&
                    li.WriteOffDate >= dateFrom &&
                    li.WriteOffDate <= dateTo);
            }

            return await query
                .OrderByDescending(li =>
                    isReceipt
                    ? li.ArrivalDate
                    : li.WriteOffDate)

                .Select(li => new FundAnalyticsDto
                {
                    InventoryNumber = li.InventoryNumber,
                    WorkTitle = li.Edition.Work.Title,
                    LibraryName = li.Hall.Library.Name,
                    HallName = li.Hall.Name,
                    ShelfCode = li.InventoryNumber,
                    Status = li.Status.Name,
                    ArrivalDate = li.ArrivalDate,
                    WriteOffDate = li.WriteOffDate
                })
                .ToListAsync();
        }
    }
}