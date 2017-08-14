using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VacationServer.Models
{
    public class VacationDbContext : DbContext
    {
        public VacationDbContext(DbContextOptions<VacationDbContext> options) : base(options)
        {
        }

        public VacationDbContext()
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<VacationDay> VacationDays { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamMember> TeamMembers { get; set; }
        public virtual DbSet<VacationConfig> VacationConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Skip shadow types
                if (entityType.ClrType == null)
                {
                    continue;
                }

                entityType.Relational().TableName = entityType.ClrType.Name;
            }

            modelBuilder.Entity<VacationDay>().HasKey("UserId", "Year");
            modelBuilder.Entity<TeamMember>().HasKey("TeamId", "MemberId");



            // check
            //modelBuilder.Entity<Account>().HasOne(a => a.LockBy);
            //modelBuilder.Entity<Account>().HasOne(a => a.CreatedBy);
            //modelBuilder.Entity<Account>().HasOne(a => a.ModifiedBy);
            //modelBuilder.Entity<AccountRole>().HasKey("AccountId", "RoleId");
            //modelBuilder.Entity<RolePermission>().HasKey("RoleId", "PermissionId");
            //modelBuilder.Entity<AccountPermission>().HasKey("AccountId", "PermissionId");


            base.OnModelCreating(modelBuilder);
        }

        public static void UpdateDatabase(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<VacationDbContext>();

            context.Database.Migrate();

            VacationDbContext.Seed(context);
        }

        private static void Seed(VacationDbContext context)
        {
            //seed code
        }
    }
}