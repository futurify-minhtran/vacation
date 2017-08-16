using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AuthenticationServer.Models;

namespace AuthenticationServer.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    [Migration("20170816043336_Add_Department_Account")]
    partial class Add_Department_Account
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AuthenticationServer.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<int>("Department");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<int>("Gender");

                    b.Property<bool>("IsSystemAdmin");

                    b.Property<string>("LastName");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<int>("Position");

                    b.Property<int>("RemainingDaysOff");

                    b.Property<string>("SecurityStamp")
                        .IsRequired();

                    b.Property<bool>("Status");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("AuthenticationServer.Models.AccountPermission", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<string>("PermissionId");

                    b.HasKey("AccountId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("AccountPermission");
                });

            modelBuilder.Entity("AuthenticationServer.Models.AccountRole", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<int>("RoleId");

                    b.HasKey("AccountId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AccountRole");
                });

            modelBuilder.Entity("AuthenticationServer.Models.Permission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Display");

                    b.HasKey("Id");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("AuthenticationServer.Models.RequestResetPassword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<DateTime>("ExpiredAt");

                    b.Property<string>("Token")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("RequestResetPassword");
                });

            modelBuilder.Entity("AuthenticationServer.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("AuthenticationServer.Models.RolePermission", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<string>("PermissionId");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermission");
                });

            modelBuilder.Entity("AuthenticationServer.Models.AccountPermission", b =>
                {
                    b.HasOne("AuthenticationServer.Models.Account", "Account")
                        .WithMany("AccountPermissions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AuthenticationServer.Models.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AuthenticationServer.Models.AccountRole", b =>
                {
                    b.HasOne("AuthenticationServer.Models.Account", "Account")
                        .WithMany("AccountRoles")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AuthenticationServer.Models.Role", "Role")
                        .WithMany("AccountRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AuthenticationServer.Models.RolePermission", b =>
                {
                    b.HasOne("AuthenticationServer.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AuthenticationServer.Models.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
