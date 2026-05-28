using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using CityLibraryFund.WPF.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services
{
    public class ReaderService
    {
        public async Task<List<string>> GetReaderTypesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.ReaderTypes
                .OrderBy(rt => rt.Name)
                .Select(rt => rt.Name)
                .ToListAsync();
        }

        public async Task<List<ReaderType>> GetReaderTypesForEditAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.ReaderTypes
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<Library>> GetLibrariesAsync()
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Libraries
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<ReaderListDto>> GetReadersAsync(
            string? search,
            string? readerType,
            bool? isActive,
            string sortBy)
        {
            using AppDbContext context = DbContextFactory.Create();

            var query = context.Readers
                .Include(r => r.ReaderType)
                .Include(r => r.Library)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(r =>
                    (r.LastName + " " +
                     r.FirstName + " " +
                     (r.Patronymic ?? ""))
                    .Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(readerType))
            {
                query = query.Where(r => r.ReaderType.Name == readerType);
            }

            if (isActive.HasValue)
            {
                query = query.Where(r => r.IsActive == isActive.Value);
            }

            query = sortBy switch
            {
                "fullNameDesc" =>
                    query.OrderByDescending(r => r.LastName)
                         .ThenByDescending(r => r.FirstName),

                "registrationDateDesc" =>
                    query.OrderByDescending(r => r.RegistrationDate),

                "registrationDateAsc" =>
                    query.OrderBy(r => r.RegistrationDate),

                _ =>
                    query.OrderBy(r => r.LastName)
                         .ThenBy(r => r.FirstName)
            };

            return await query
                .Select(r => new ReaderListDto
                {
                    Id = r.Id,

                    FullName =
                        r.LastName + " " +
                        r.FirstName + " " +
                        (r.Patronymic ?? ""),

                    ReaderType = r.ReaderType.Name,
                    Phone = r.Phone,
                    LibraryName = r.Library.Name,
                    RegistrationDate = r.RegistrationDate,
                    IsActive = r.IsActive
                })
                .ToListAsync();
        }

        public async Task<ReaderDetailsDto?> GetReaderDetailsAsync(int readerId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var reader = await context.Readers
                .Include(r => r.ReaderType)
                .Include(r => r.Library)
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.Scientist)
                .Include(r => r.Schooler)
                .FirstOrDefaultAsync(r => r.Id == readerId);

            if (reader == null)
            {
                return null;
            }

            return new ReaderDetailsDto
            {
                Id = reader.Id,

                FullName =
                    reader.LastName + " " +
                    reader.FirstName + " " +
                    (reader.Patronymic ?? ""),

                ReaderType = reader.ReaderType.Name,
                BirthDate = reader.BirthDate,
                RegistrationDate = reader.RegistrationDate,

                Phone = reader.Phone,
                Email = reader.Email,
                Address = reader.Address,

                LibraryName = reader.Library.Name,
                IsActive = reader.IsActive,

                University =
                    reader.Student?.University ??
                    reader.Teacher?.University,

                Faculty = reader.Student?.Faculty,
                Course = reader.Student?.Course,
                GroupName = reader.Student?.GroupName,

                Department = reader.Teacher?.Department,

                Organization = reader.Scientist?.Organization,
                ScientificTopic = reader.Scientist?.ScientificTopic,

                School = reader.Schooler?.School
            };
        }

        public async Task<ReaderEditDto?> GetReaderForEditAsync(int readerId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var reader = await context.Readers
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.Scientist)
                .Include(r => r.Schooler)
                .FirstOrDefaultAsync(r => r.Id == readerId);

            if (reader == null)
            {
                return null;
            }

            return new ReaderEditDto
            {
                Id = reader.Id,

                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Patronymic = reader.Patronymic,

                BirthDate = reader.BirthDate,

                ReaderTypeId = reader.ReaderTypeId,
                LibraryId = reader.LibraryId,

                Phone = reader.Phone,
                Email = reader.Email,
                Address = reader.Address,

                IsActive = reader.IsActive,

                University =
                    reader.Student?.University ??
                    reader.Teacher?.University,

                Faculty = reader.Student?.Faculty,
                Course = reader.Student?.Course,
                GroupName = reader.Student?.GroupName,

                Department = reader.Teacher?.Department,

                Organization = reader.Scientist?.Organization,
                ScientificTopic = reader.Scientist?.ScientificTopic,

                School = reader.Schooler?.School,
                ClassNumber = reader.Schooler?.ClassNumber
            };
        }

        public async Task<OperationResultDto> CreateReaderAsync(ReaderEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();

            dto.Patronymic = dto.Patronymic?.Trim();

            dto.Phone = dto.Phone?.Trim();
            dto.Email = dto.Email?.Trim();
            dto.Address = dto.Address?.Trim();

            dto.University = dto.University?.Trim();
            dto.Faculty = dto.Faculty?.Trim();
            dto.GroupName = dto.GroupName?.Trim();

            dto.Department = dto.Department?.Trim();

            dto.Organization = dto.Organization?.Trim();
            dto.ScientificTopic = dto.ScientificTopic?.Trim();

            dto.School = dto.School?.Trim();

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

            if (!ValidationHelper.IsValidEmail(dto.Email))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный email."
                };
            }

            if (!ValidationHelper.IsValidPhone(dto.Phone))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный номер телефона."
                };
            }

            if (dto.Phone?.Length > 20)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Телефон слишком длинный."
                };
            }

            if (dto.Email?.Length > 100)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Email слишком длинный."
                };
            }

            Reader reader = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Patronymic = dto.Patronymic,

                BirthDate = dto.BirthDate,

                ReaderTypeId = dto.ReaderTypeId,
                LibraryId = dto.LibraryId,

                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,

                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true
            };

            context.Readers.Add(reader);

            await context.SaveChangesAsync();

            string readerType = await context.ReaderTypes
                .Where(x => x.Id == dto.ReaderTypeId)
                .Select(x => x.Name)
                .FirstAsync();

            if (readerType == "Студент")
            {
                if (string.IsNullOrWhiteSpace(dto.University))
                {
                    return new OperationResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Введите ВУЗ."
                    };
                }

                context.Students.Add(new Student
                {
                    ReaderId = reader.Id,
                    University = dto.University,
                    Faculty = dto.Faculty ?? "",
                    Course = dto.Course ?? 1,
                    GroupName = dto.GroupName ?? ""
                });
            }
            else if (readerType == "Преподаватель")
            {
                context.Teachers.Add(new Teacher
                {
                    ReaderId = reader.Id,
                    University = dto.University ?? "",
                    Department = dto.Department ?? ""
                });
            }
            else if (readerType == "Научный работник")
            {
                context.Scientists.Add(new Scientist
                {
                    ReaderId = reader.Id,
                    Organization = dto.Organization ?? "",
                    ScientificTopic = dto.ScientificTopic ?? ""
                });
            }
            else if (readerType == "Школьник")
            {
                context.Schoolers.Add(new Schooler
                {
                    ReaderId = reader.Id,
                    School = dto.School ?? "",
                    ClassNumber = dto.ClassNumber ?? 1
                });
            }

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<OperationResultDto> UpdateReaderAsync(ReaderEditDto dto)
        {
            using AppDbContext context = DbContextFactory.Create();

            var reader = await context.Readers
                .Include(r => r.Student)
                .Include(r => r.Teacher)
                .Include(r => r.Scientist)
                .Include(r => r.Schooler)
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

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

            dto.FirstName = dto.FirstName.Trim();
            dto.LastName = dto.LastName.Trim();

            dto.Patronymic = dto.Patronymic?.Trim();

            dto.Phone = dto.Phone?.Trim();
            dto.Email = dto.Email?.Trim();
            dto.Address = dto.Address?.Trim();

            dto.University = dto.University?.Trim();
            dto.Faculty = dto.Faculty?.Trim();
            dto.GroupName = dto.GroupName?.Trim();

            dto.Department = dto.Department?.Trim();

            dto.Organization = dto.Organization?.Trim();
            dto.ScientificTopic = dto.ScientificTopic?.Trim();

            dto.School = dto.School?.Trim();

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

            if (!ValidationHelper.IsValidEmail(dto.Email))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный email."
                };
            }

            if (!ValidationHelper.IsValidPhone(dto.Phone))
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Некорректный номер телефона."
                };
            }

            if (dto.Phone?.Length > 20)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Телефон слишком длинный."
                };
            }

            if (dto.Email?.Length > 100)
            {
                return new OperationResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Email слишком длинный."
                };
            }

            reader.FirstName = dto.FirstName;
            reader.LastName = dto.LastName;
            reader.Patronymic = dto.Patronymic;

            reader.BirthDate = dto.BirthDate;
            reader.LibraryId = dto.LibraryId;

            reader.Phone = dto.Phone;
            reader.Email = dto.Email;
            reader.Address = dto.Address;

            if (reader.Student != null)
            {
                reader.Student.University = dto.University ?? "";
                reader.Student.Faculty = dto.Faculty ?? "";
                reader.Student.Course = dto.Course ?? 1;
                reader.Student.GroupName = dto.GroupName ?? "";
            }

            if (reader.Teacher != null)
            {
                reader.Teacher.University = dto.University ?? "";
                reader.Teacher.Department = dto.Department ?? "";
            }

            if (reader.Scientist != null)
            {
                reader.Scientist.Organization = dto.Organization ?? "";
                reader.Scientist.ScientificTopic = dto.ScientificTopic ?? "";
            }

            if (reader.Schooler != null)
            {
                reader.Schooler.School = dto.School ?? "";
                reader.Schooler.ClassNumber = dto.ClassNumber ?? 1;
            }

            await context.SaveChangesAsync();

            return new OperationResultDto
            {
                IsSuccess = true
            };
        }

        public async Task<bool> ToggleReaderActivityAsync(int readerId)
        {
            using AppDbContext context = DbContextFactory.Create();

            var reader = await context.Readers
                .FirstOrDefaultAsync(r => r.Id == readerId);

            if (reader == null)
            {
                return false;
            }

            reader.IsActive = !reader.IsActive;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<LoanHistoryDto>> GetReaderLoanHistoryAsync(int readerId)
        {
            using AppDbContext context = DbContextFactory.Create();

            return await context.Loans
                .Include(l => l.LibraryItem)
                    .ThenInclude(li => li.Edition)
                        .ThenInclude(e => e.Work)
                .Where(l => l.ReaderId == readerId)
                .OrderByDescending(l => l.IssueDate)
                .Select(l => new LoanHistoryDto
                {
                    Id = l.Id,

                    BookTitle = l.LibraryItem.Edition.Work.Title,

                    LoanDate = l.IssueDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,

                    IsReturned = l.ReturnDate != null,

                    IsOverdue =
                        l.ReturnDate == null &&
                        l.DueDate < DateOnly.FromDateTime(DateTime.Now)
                })
                .ToListAsync();
        }
    }
}