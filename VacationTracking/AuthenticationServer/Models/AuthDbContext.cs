using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using AuthenticationServer.Services;
using AuthenticationServer.Setup;

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
            if (!context.Accounts.Any())
            {
                var now = DateTime.Now;

                var account = new Account()
                {
                    Email = "tranquocminh1112@gmail.com",
                    FirstName = "Quoc Minh",
                    LastName = "Tran",
                    Gender = Gender.Male,
                    IsSystemAdmin = true,
                    PhoneNumber = "0906901112",
                    DateOfBirth = new DateTime(1990, 12, 11),
                    Position = Position.Development,
                    Department = Department.Outsource,
                    Status = true,
                    Password = "123",
                    SecurityStamp = AccountService.GenerateSecurityStamp(),
                    CreatedAt = now,
                    ModifiedAt = now
                };

                account.Password = (new PasswordHasher<Account>()).HashPassword(account, account.Password);

                account.AccountPermissions = new List<AccountPermission>()
                {
                    new AccountPermission()
                    {
                        PermissionId = PermissionsList.ADMIN_PERMISSION
                    }
                };

                context.Accounts.Add(account);

                context.SaveChanges();
            }
        }
    }
}
