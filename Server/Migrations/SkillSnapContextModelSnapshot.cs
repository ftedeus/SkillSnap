﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillSnap.Server.Data;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(SkillSnapContext))]
    partial class SkillSnapContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.6");

            modelBuilder.Entity("SkillSnap.Shared.Models.PortfolioUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileImageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("PortfolioUsers");
                });

            modelBuilder.Entity("SkillSnap.Shared.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<int>("PortfolioUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioUserId", "Title")
                        .IsUnique();

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("SkillSnap.Shared.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PortfolioUserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioUserId");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("SkillSnap.Shared.Models.Project", b =>
                {
                    b.HasOne("SkillSnap.Shared.Models.PortfolioUser", "PortfolioUser")
                        .WithMany("Projects")
                        .HasForeignKey("PortfolioUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PortfolioUser");
                });

            modelBuilder.Entity("SkillSnap.Shared.Models.Skill", b =>
                {
                    b.HasOne("SkillSnap.Shared.Models.PortfolioUser", "PortfolioUser")
                        .WithMany("Skills")
                        .HasForeignKey("PortfolioUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PortfolioUser");
                });

            modelBuilder.Entity("SkillSnap.Shared.Models.PortfolioUser", b =>
                {
                    b.Navigation("Projects");

                    b.Navigation("Skills");
                });
#pragma warning restore 612, 618
        }
    }
}
