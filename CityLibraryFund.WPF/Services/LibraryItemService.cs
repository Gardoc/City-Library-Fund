using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class LibraryItemService
    {
        public async Task<List<LibraryItemStatus>> GetStatusesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.LibraryItemStatuses
                .OrderBy(s => s.Name)
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
                .OrderBy(h => h.Library.Name)
                .ThenBy(h => h.Name)
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

        public async Task<List<Edition>> GetEditionsAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Editions
                .Include(e => e.Work)
                .OrderBy(e => e.Work.Title)
                .ToListAsync();
        }

        public async Task<List<LibraryItemListDto>> GetLibraryItemsAsync(
            string? search,
            int? statusId,
            bool? onlyReadingRoom,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.LibraryItems
                .Include(li => li.Edition)
                    .ThenInclude(e => e.Work)
                .Include(li => li.Hall)
                    .ThenInclude(h => h.Library)
                .Include(li => li.Status)
                .AsQueryable();

            if (CurrentUserService.Role == "Библиотекарь")
            {
                query = query.Where(li =>
                    li.Hall.LibraryId == CurrentUserService.LibraryId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(li =>
                    li.InventoryNumber.Contains(search) ||
                    li.Edition.Work.Title.Contains(search));
            }

            if (statusId.HasValue)
            {
                query = query.Where(li => li.StatusId == statusId.Value);
            }

            if (onlyReadingRoom.HasValue)
            {
                query = query.Where(li =>
                    li.OnlyReadingRoom == onlyReadingRoom.Value);
            }

            query = sortBy switch
            {
                "inventoryDesc" =>
                    query.OrderByDescending(li => li.InventoryNumber),

                "arrivalDateAsc" =>
                    query.OrderBy(li => li.ArrivalDate),

                "arrivalDateDesc" =>
                    query.OrderByDescending(li => li.ArrivalDate),

                _ =>
                    query.OrderBy(li => li.InventoryNumber)
            };

            return await query
                .Select(li => new LibraryItemListDto
                {
                    Id = li.Id,
                    InventoryNumber = li.InventoryNumber,
                    WorkTitle = li.Edition.Work.Title,
                    Publisher = li.Edition.Publisher,
                    PublishYear = li.Edition.PublishYear,
                    LibraryName = li.Hall.Library.Name,
                    HallName = li.Hall.Name,
                    Status = li.Status.Name,
                    OnlyReadingRoom = li.OnlyReadingRoom
                })
                .ToListAsync();
        }

        public async Task<LibraryItemDetailsDto?> GetLibraryItemDetailsAsync(int libraryItemId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var item = await context.LibraryItems
                .Include(li => li.Edition)
                    .ThenInclude(e => e.Work)
                        .ThenInclude(w => w.Authors)
                .Include(li => li.Hall)
                    .ThenInclude(h => h.Library)
                .Include(li => li.Status)
                .Include(li => li.Loans)
                .FirstOrDefaultAsync(li => li.Id == libraryItemId);

            if (item == null)
            {
                return null;
            }

            return new LibraryItemDetailsDto
            {
                Id = item.Id,
                InventoryNumber = item.InventoryNumber,
                WorkTitle = item.Edition.Work.Title,

                Authors = string.Join(
                    ", ",
                    item.Edition.Work.Authors
                        .OrderBy(a => a.LastName)
                        .Select(a => a.LastName + " " + a.FirstName)),

                Publisher = item.Edition.Publisher,
                PublishYear = item.Edition.PublishYear,
                Isbn = item.Edition.Isbn,
                LibraryName = item.Hall.Library.Name,
                HallName = item.Hall.Name,
                RackNumber = item.RackNumber,
                ShelfNumber = item.ShelfNumber,
                Status = item.Status.Name,
                OnlyReadingRoom = item.OnlyReadingRoom,
                LoanDays = item.LoanDays,
                ArrivalDate = item.ArrivalDate,
                WriteOffDate = item.WriteOffDate,
                Price = item.Price,
                IsWrittenOff = item.WriteOffDate != null,
                IsIssued = item.Loans.Any(l => l.ReturnDate == null)
            };
        }

        public async Task<LibraryItemEditDto?> GetLibraryItemForEditAsync(int libraryItemId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var item = await context.LibraryItems
                .FirstOrDefaultAsync(li => li.Id == libraryItemId);

            if (item == null)
            {
                return null;
            }

            return new LibraryItemEditDto
            {
                Id = item.Id,
                EditionId = item.EditionId,
                HallId = item.HallId,
                InventoryNumber = item.InventoryNumber,
                RackNumber = item.RackNumber,
                ShelfNumber = item.ShelfNumber,
                OnlyReadingRoom = item.OnlyReadingRoom,
                LoanDays = item.LoanDays,
                ArrivalDate = item.ArrivalDate,
                WriteOffDate = item.WriteOffDate,
                Price = item.Price
            };
        }

        public async Task<OperationResultDto> CreateLibraryItemAsync(LibraryItemEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            dto.InventoryNumber = dto.InventoryNumber.Trim();

            if (string.IsNullOrWhiteSpace(dto.InventoryNumber))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите инвентарный номер."
                };
            }

            bool inventoryExists = await context.LibraryItems
                .AnyAsync(li => li.InventoryNumber == dto.InventoryNumber);

            if (inventoryExists)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Инвентарный номер уже существует."
                };
            }

            if (dto.RackNumber <= 0)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный номер стеллажа."
                };
            }

            if (dto.ShelfNumber <= 0)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный номер полки."
                };
            }

            if (dto.Price < 0)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Цена не может быть отрицательной."
                };
            }

            if (dto.ArrivalDate > DateOnly.FromDateTime(DateTime.Now))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Дата поступления некорректна."
                };
            }

            if (dto.OnlyReadingRoom)
            {
                dto.LoanDays = null;
            }
            else
            {
                if (dto.LoanDays == null || dto.LoanDays <= 0)
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Укажите срок выдачи."
                    };
                }
            }

            var availableStatus = await context.LibraryItemStatuses
                .FirstOrDefaultAsync(s => s.Name == "В наличии");

            if (availableStatus == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Статус 'В наличии' не найден."
                };
            }

            LibraryItem item = new()
            {
                EditionId = dto.EditionId,
                HallId = dto.HallId,
                StatusId = availableStatus.Id,
                InventoryNumber = dto.InventoryNumber,
                RackNumber = dto.RackNumber,
                ShelfNumber = dto.ShelfNumber,
                OnlyReadingRoom = dto.OnlyReadingRoom,
                LoanDays = dto.LoanDays,
                ArrivalDate = dto.ArrivalDate,
                WriteOffDate = dto.WriteOffDate,
                Price = dto.Price
            };

            context.LibraryItems.Add(item);

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> UpdateLibraryItemAsync(LibraryItemEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var item = await context.LibraryItems
                .FirstOrDefaultAsync(li => li.Id == dto.Id);

            if (item == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Экземпляр не найден."
                };
            }

            dto.InventoryNumber = dto.InventoryNumber.Trim();

            if (string.IsNullOrWhiteSpace(dto.InventoryNumber))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Введите инвентарный номер."
                };
            }

            bool inventoryExists = await context.LibraryItems
                .AnyAsync(li =>
                    li.InventoryNumber == dto.InventoryNumber &&
                    li.Id != dto.Id);

            if (inventoryExists)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Инвентарный номер уже существует."
                };
            }

            if (dto.RackNumber <= 0)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный номер стеллажа."
                };
            }

            if (dto.ShelfNumber <= 0)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный номер полки."
                };
            }

            if (dto.Price < 0)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Цена не может быть отрицательной."
                };
            }

            if (dto.ArrivalDate > DateOnly.FromDateTime(DateTime.Now))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Дата поступления некорректна."
                };
            }

            if (dto.OnlyReadingRoom)
            {
                dto.LoanDays = null;
            }
            else
            {
                if (dto.LoanDays == null || dto.LoanDays <= 0)
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Укажите срок выдачи."
                    };
                }
            }

            item.EditionId = dto.EditionId;
            item.HallId = dto.HallId;
            item.InventoryNumber = dto.InventoryNumber;
            item.RackNumber = dto.RackNumber;
            item.ShelfNumber = dto.ShelfNumber;
            item.OnlyReadingRoom = dto.OnlyReadingRoom;
            item.LoanDays = dto.LoanDays;
            item.ArrivalDate = dto.ArrivalDate;
            item.WriteOffDate = dto.WriteOffDate;
            item.Price = dto.Price;

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> WriteOffLibraryItemAsync(int libraryItemId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var item = await context.LibraryItems
                .Include(li => li.Loans)
                .FirstOrDefaultAsync(li => li.Id == libraryItemId);

            if (item == null)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Экземпляр не найден."
                };
            }

            bool hasActiveLoan = item.Loans.Any(l => l.ReturnDate == null);

            if (hasActiveLoan)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage =
                        "Нельзя списать экземпляр, который находится на руках."
                };
            }

            item.WriteOffDate = DateOnly.FromDateTime(DateTime.Now);

            var writeOffStatus = await context.LibraryItemStatuses
                .FirstOrDefaultAsync(s => s.Name == "Списано");

            if (writeOffStatus != null)
            {
                item.StatusId = writeOffStatus.Id;
            }

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }
    }
}