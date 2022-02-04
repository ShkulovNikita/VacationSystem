using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationSystem.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace VacationSystem
{
    public class ApplicationContext : DbContext
    {
        // файл логов БД
        private readonly StreamWriter logStream = new StreamWriter("logs.txt", true);
 
        // таблицы БД
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
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<EmployeeInGroup> EmployeeInGroups { get; set; }

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ключи для слабых сущностей БД

            modelBuilder.Entity<EmployeeInRule>().HasKey(e => new { e.EmployeeId, e.EmployeeRuleId });

            modelBuilder.Entity<EmployeeInGroup>().HasKey(e => new { e.EmployeeId, e.GroupId });

            modelBuilder.Entity<HeadStyle>().HasKey(s => new { s.DepartmentId, s.EmployeeId, s.ManagementStyleId });

            modelBuilder.Entity<Deputy>().HasKey(d => new { d.HeadEmployeeId, d.DeputyEmployeeId, d.DepartmentId });
            
            /*несколько связей к одной и той же таблице*/

            // VisibilityForEmployee и Employee
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

            // Deputy и Employee
            modelBuilder.Entity<Deputy>()
                .HasOne(m => m.HeadEmployee)
                .WithMany(t => t.DeputyHeads)
                .HasForeignKey(m => m.HeadEmployeeId);

            modelBuilder.Entity<Deputy>()
                .HasOne(m => m.DeputyEmployee)
                .WithMany(t => t.DeputyEmployees)
                .HasForeignKey(m => m.DeputyEmployeeId);
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

        public void RecreateDatabase()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}