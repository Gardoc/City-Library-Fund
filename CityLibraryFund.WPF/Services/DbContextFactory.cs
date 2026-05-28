using System.Configuration;
using CityLibraryFund.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.WPF.Services;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        var connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}