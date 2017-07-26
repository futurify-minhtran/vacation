using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationServer.Models
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public AuthDbContext()
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AccountRole> AccountsRoles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<AccountPermission> AccountsPermissions { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<RequestResetPassword> RequestResetPasswords { get; set; }

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
            // check
            //modelBuilder.Entity<Account>().HasOne(a => a.LockBy);
            //modelBuilder.Entity<Account>().HasOne(a => a.CreatedBy);
            //modelBuilder.Entity<Account>().HasOne(a => a.ModifiedBy);
            modelBuilder.Entity<AccountRole>().HasKey("AccountId", "RoleId");
            modelBuilder.Entity<RolePermission>().HasKey("RoleId", "PermissionId");
            modelBuilder.Entity<AccountPermission>().HasKey("AccountId", "PermissionId");

            base.OnModelCreating(modelBuilder);
        }

        public static void UpdateDatabase(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<AuthDbContext>();

            context.Database.Migrate();

            AuthDbContext.Seed(context);
        }

        private static void Seed(AuthDbContext context)
        {
            //seed code
        }
    }
}
