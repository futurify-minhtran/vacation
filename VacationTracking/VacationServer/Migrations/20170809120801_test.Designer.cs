using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VacationServer.Models;

namespace VacationServer.Migrations
{
    [DbContext(typeof(VacationDbContext))]
    [Migration("20170809120801_test")]
    partial class test
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VacationServer.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BookingType");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("ModifiedAt");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Booking");
                });

            modelBuilder.Entity("VacationServer.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("LeaderId");

                    b.Property<DateTime>("ModifiedAt");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("VacationServer.Models.TeamMember", b =>
                {
                    b.Property<int?>("TeamId");

                    b.Property<int>("MemberId");

                    b.HasKey("TeamId", "MemberId");

                    b.ToTable("TeamMember");
                });

            modelBuilder.Entity("VacationServer.Models.VacationDay", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("Year");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("ModifiedAt");

                    b.Property<int>("TotalMonth");

                    b.HasKey("UserId", "Year");

                    b.ToTable("VacationDay");
                });

            modelBuilder.Entity("VacationServer.Models.TeamMember", b =>
                {
                    b.HasOne("VacationServer.Models.Team", "Team")
                        .WithMany("TeamMembers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
