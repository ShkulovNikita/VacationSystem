using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationSystem.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter logStream = new StreamWriter("logs.txt", true);
 
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<RuleType> RuleTypes { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<RuleForPosition> RuleForPositions { get; set; }
        public DbSet<ForbiddenPeriod> ForbiddenPeriods { get; set; }
        public DbSet<ChoicePeriod> ChoicePeriods { get; set; }
        public DbSet<VacationStatus> VacationStatuses { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }
        public DbSet<ManagementStyle> ManagementStyles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<VisibilityForDepartment> VisibilityForDepartments { get; set; }
        public DbSet<HeadStyle> HeadStyles { get; set; }
        public DbSet<SetVacation> SetVacations { get; set; }
        public DbSet<WishedVacationPeriod> WishedVacationPeriods { get; set; }
        public DbSet<VacationDay> VacationDays { get; set; }
        public DbSet<VisibilityForEmployee> VisibilityForEmployees { get; set; }
        public DbSet<EmployeeRule> EmployeeRules { get; set; }
        public DbSet<EmployeeInRule> EmployeeInRules { get; set; }

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeInRule>().HasKey(e => new { e.EmployeeId, e.EmployeeRuleId });
            
            modelBuilder.Entity<HeadStyle>().HasKey(s => new { s.DepartmentId, s.EmployeeId, s.ManagementStyleId });
            
            modelBuilder.Entity<VisibilityForEmployee>()
                .HasOne(m => m.VisibilityEmployee)
                .WithMany(t => t.VisibilityEmployees)
                .HasForeignKey(m => m.VisibilityEmployeeId);

            modelBuilder.Entity<VisibilityForEmployee>()
                .HasOne(m => m.TargetEmployee)
                .WithMany(t => t.VisibilityTargets)
                .HasForeignKey(m => m.TargetEmployeeId);
            
            modelBuilder.Entity<VisibilityForEmployee>()
                .HasOne(m => m.HeadEmployee)
                .WithMany(t => t.VisibilityHeads)
                .HasForeignKey(m => m.HeadEmployeeId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получение конфигурации из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создание конфигурации
            var config = builder.Build();
            // получение строки подключения
            string connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(5, 7, 35))
                );
            optionsBuilder.LogTo(logStream.WriteLine);
        }

        public override void Dispose()
        {
            base.Dispose();
            logStream.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            await logStream.DisposeAsync();
        }
    }
}