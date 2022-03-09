using System;
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
        //private readonly StreamWriter logStream = new StreamWriter("logs.txt", true);
 
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
        public DbSet<GroupRule> GroupRules { get; set; }
        public DbSet<IndividualChoicePeriod> IndividualChoicePeriods { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ключи для слабых сущностей БД

            modelBuilder.Entity<EmployeeInRule>().HasKey(e => new { e.EmployeeId, e.EmployeeRuleId });

            modelBuilder.Entity<EmployeeInGroup>().HasKey(e => new { e.EmployeeId, e.GroupId });

            modelBuilder.Entity<HeadStyle>().HasKey(s => new { s.DepartmentId, s.HeadEmployeeId, s.ManagementStyleId });

            modelBuilder.Entity<ChoicePeriod>().HasKey(c => new { c.StartDate, c.DepartmentId });
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
                new MySqlServerVersion(new Version(8, 0, 28))
                );
            //optionsBuilder.LogTo(logStream.WriteLine);
        }

        public override void Dispose()
        {
            base.Dispose();
            //logStream.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            //await logStream.DisposeAsync();
        }

        public void RecreateDatabase()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public void DeleteDatabase()
        {
            Database.EnsureDeleted();
        }
    }
}