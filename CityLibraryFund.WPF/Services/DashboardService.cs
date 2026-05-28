using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityLibraryFund.WPF.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VwDashboardStatistic?> GetStatisticsAsync()
        {
            return await _context.VwDashboardStatistics
                .FirstOrDefaultAsync();
        }

        public async Task<List<VwCurrentLoan>> GetCurrentLoansAsync()
        {
            return await _context.VwCurrentLoans
                .OrderByDescending(x => x.IssueDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<VwOverdueReader>> GetOverdueReadersAsync()
        {
            return await _context.VwOverdueReaders
                .OrderByDescending(x => x.OverdueDays)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<VwPopularWork>> GetPopularWorksAsync()
        {
            return await _context.VwPopularWorks
                .OrderByDescending(x => x.LoanCount)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<VwEmployeePerformance>> GetEmployeePerformanceAsync()
        {
            return await _context.VwEmployeePerformances
                .OrderByDescending(x => x.ReadersServed)
                .Take(10)
                .ToListAsync();
        }
    }
}