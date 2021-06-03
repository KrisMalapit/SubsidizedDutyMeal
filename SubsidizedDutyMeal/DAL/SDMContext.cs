using SubsidizedDutyMeal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SubsidizedDutyMeal.DAL
{
    public class SDMContext : DbContext
    {
        public SDMContext() : base("SDMContext")
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<EmploymentStatus> EmploymentStatus { get; set; }
        public DbSet<ScheduleHeader> ScheduleHeaders { get; set; }

        public DbSet<ScheduleDetail> ScheduleDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<NoSeries> NoSeries { get; set; }
        public DbSet<Concessionaire> Concessionaires { get; set; }
        public DbSet<MealHeader> MealHeaders { get; set; }
        public DbSet<MealDetails> MealDetails { get; set; }

        public DbSet<BiometricsData> BiometricsDatas { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OverrideOrder> OverrideOrders { get; set; }

        public DbSet<VoidMeal> VoidMeals { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<BiometricIP> BiometricIPs { get; set; }

        public DbSet<MealOverRide> MealOverRide { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<ScheduleMatrix> ScheduleMatrixes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TempTable> TempTables { get; set;   }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}