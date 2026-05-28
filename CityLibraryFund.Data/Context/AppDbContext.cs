using System;
using System.Collections.Generic;
using CityLibraryFund.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityLibraryFund.Data.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Edition> Editions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<HallType> HallTypes { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<LibraryItem> LibraryItems { get; set; }

    public virtual DbSet<LibraryItemStatus> LibraryItemStatuses { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<ReaderType> ReaderTypes { get; set; }

    public virtual DbSet<Schooler> Schoolers { get; set; }

    public virtual DbSet<Scientist> Scientists { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    public virtual DbSet<VwCurrentLoan> VwCurrentLoans { get; set; }

    public virtual DbSet<VwDashboardStatistic> VwDashboardStatistics { get; set; }

    public virtual DbSet<VwEmployeePerformance> VwEmployeePerformances { get; set; }

    public virtual DbSet<VwInactiveReader> VwInactiveReaders { get; set; }

    public virtual DbSet<VwOverdueReader> VwOverdueReaders { get; set; }

    public virtual DbSet<VwPopularWork> VwPopularWorks { get; set; }

    public virtual DbSet<Work> Works { get; set; }

    public virtual DbSet<WorkType> WorkTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Author__3214EC0786465690");

            entity.ToTable("Author", tb =>
                tb.HasTrigger("TR_Author_PreventDelete"));
        });

        modelBuilder.Entity<Edition>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Edition__3214EC07937E8686");

            entity.ToTable("Edition", tb =>
                tb.HasTrigger("TR_Edition_PreventDelete"));

            entity.HasOne(d => d.Work)
                .WithMany(p => p.Editions)
                .HasConstraintName("FK_Edition_Work");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Employee__3214EC071F530968");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.HasOne(d => d.EmployeeRole)
                .WithMany(p => p.Employees)
                .HasConstraintName("FK_Employee_EmployeeRole");

            entity.HasOne(d => d.Hall)
                .WithMany(p => p.Employees)
                .HasConstraintName("FK_Employee_Hall");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Employee__3214EC07A01B727F");
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Hall__3214EC075A19BC18");

            entity.HasOne(d => d.HallType)
                .WithMany(p => p.Halls)
                .HasConstraintName("FK_Hall_HallType");

            entity.HasOne(d => d.Library)
                .WithMany(p => p.Halls)
                .HasConstraintName("FK_Hall_Library");
        });

        modelBuilder.Entity<HallType>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__HallType__3214EC0781C6F01B");
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Library__3214EC075C63289E");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<LibraryItem>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__LibraryI__3214EC07374BA7A3");

            entity.ToTable("LibraryItem", tb =>
            {
                tb.HasTrigger("TR_LibraryItem_PreventDelete");
                tb.HasTrigger("TR_LibraryItem_PreventWriteOffIssued");
            });

            entity.HasOne(d => d.Edition)
                .WithMany(p => p.LibraryItems)
                .HasConstraintName("FK_LibraryItem_Edition");

            entity.HasOne(d => d.Hall)
                .WithMany(p => p.LibraryItems)
                .HasConstraintName("FK_LibraryItem_Hall");

            entity.HasOne(d => d.Status)
                .WithMany(p => p.LibraryItems)
                .HasConstraintName("FK_LibraryItem_Status");
        });

        modelBuilder.Entity<LibraryItemStatus>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__LibraryI__3214EC07D7C4D11D");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Loan__3214EC070E9E613F");

            entity.ToTable("Loan", tb =>
            {
                tb.HasTrigger("TR_Loan_CheckActiveLoan");
                tb.HasTrigger("TR_Loan_CheckReturnDate");
            });

            entity.HasIndex(e => e.LibraryItemId, "UX_Loan_ActiveLoan")
                .IsUnique()
                .HasFilter("([ReturnDate] IS NULL)");

            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Loans)
                .HasConstraintName("FK_Loan_Employee");

            entity.HasOne(d => d.LibraryItem)
                .WithMany(p => p.Loans)
                .HasConstraintName("FK_Loan_LibraryItem");

            entity.HasOne(d => d.Reader)
                .WithMany(p => p.Loans)
                .HasConstraintName("FK_Loan_Reader");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Reader__3214EC07A2BBC129");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Library)
                .WithMany(p => p.Readers)
                .HasConstraintName("FK_Reader_Library");

            entity.HasOne(d => d.ReaderType)
                .WithMany(p => p.Readers)
                .HasConstraintName("FK_Reader_ReaderType");
        });

        modelBuilder.Entity<ReaderType>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__ReaderTy__3214EC071C13AE7B");
        });

        modelBuilder.Entity<Schooler>(entity =>
        {
            entity.HasKey(e => e.ReaderId)
                .HasName("PK__Schooler__8E67A5E1436023E2");

            entity.Property(e => e.ReaderId)
                .ValueGeneratedNever();

            entity.HasOne(d => d.Reader)
                .WithOne(p => p.Schooler)
                .HasConstraintName("FK_Schooler_Reader");
        });

        modelBuilder.Entity<Scientist>(entity =>
        {
            entity.HasKey(e => e.ReaderId)
                .HasName("PK__Scientis__8E67A5E1F7AD2681");

            entity.Property(e => e.ReaderId)
                .ValueGeneratedNever();

            entity.HasOne(d => d.Reader)
                .WithOne(p => p.Scientist)
                .HasConstraintName("FK_Scientist_Reader");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.ReaderId)
                .HasName("PK__Student__8E67A5E1F5EC57D9");

            entity.Property(e => e.ReaderId)
                .ValueGeneratedNever();

            entity.HasOne(d => d.Reader)
                .WithOne(p => p.Student)
                .HasConstraintName("FK_Student_Reader");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.ReaderId)
                .HasName("PK__Teacher__8E67A5E1E1F4330D");

            entity.Property(e => e.ReaderId)
                .ValueGeneratedNever();

            entity.HasOne(d => d.Reader)
                .WithOne(p => p.Teacher)
                .HasConstraintName("FK_Teacher_Reader");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Visit__3214EC07D1B95E3C");

            entity.Property(e => e.VisitDate)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Visits)
                .HasConstraintName("FK_Visit_Employee");

            entity.HasOne(d => d.Hall)
                .WithMany(p => p.Visits)
                .HasConstraintName("FK_Visit_Hall");

            entity.HasOne(d => d.Reader)
                .WithMany(p => p.Visits)
                .HasConstraintName("FK_Visit_Reader");
        });

        modelBuilder.Entity<VwCurrentLoan>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_CurrentLoans");
        });

        modelBuilder.Entity<VwDashboardStatistic>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_DashboardStatistics");
        });

        modelBuilder.Entity<VwEmployeePerformance>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_EmployeePerformance");
        });

        modelBuilder.Entity<VwInactiveReader>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_InactiveReaders");
        });

        modelBuilder.Entity<VwOverdueReader>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_OverdueReaders");
        });

        modelBuilder.Entity<VwPopularWork>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_PopularWorks");
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Work__3214EC07FAD462D2");

            entity.ToTable("Work", tb =>
                tb.HasTrigger("TR_Work_PreventDelete"));

            entity.HasOne(d => d.WorkType)
                .WithMany(p => p.Works)
                .HasConstraintName("FK_Work_WorkType");

            entity.HasMany(d => d.Authors)
                .WithMany(p => p.Works)
                .UsingEntity<Dictionary<string, object>>(
                    "WorkAuthor",
                    r => r.HasOne<Author>()
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("FK_WorkAuthor_Author"),

                    l => l.HasOne<Work>()
                        .WithMany()
                        .HasForeignKey("WorkId")
                        .HasConstraintName("FK_WorkAuthor_Work"),

                    j =>
                    {
                        j.HasKey("WorkId", "AuthorId");

                        j.ToTable("WorkAuthor");
                    });
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__WorkType__3214EC079DFE1CE7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}