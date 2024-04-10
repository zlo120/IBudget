﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("Core.Model.Expense", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ID");

                    b.Property<double>("Amount")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Frequency")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("Core.Model.Income", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ID");

                    b.Property<double>("Amount")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Frequency")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Source")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Income");
                });

            modelBuilder.Entity("Core.Model.Tag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("ExpenseTag", b =>
                {
                    b.Property<int>("ExpensesID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagsID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ExpensesID", "TagsID");

                    b.HasIndex("TagsID");

                    b.ToTable("ExpenseTag");
                });

            modelBuilder.Entity("IncomeTag", b =>
                {
                    b.Property<int>("IncomesID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagsID")
                        .HasColumnType("INTEGER");

                    b.HasKey("IncomesID", "TagsID");

                    b.HasIndex("TagsID");

                    b.ToTable("IncomeTag");
                });

            modelBuilder.Entity("ExpenseTag", b =>
                {
                    b.HasOne("Core.Model.Expense", null)
                        .WithMany()
                        .HasForeignKey("ExpensesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Model.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IncomeTag", b =>
                {
                    b.HasOne("Core.Model.Income", null)
                        .WithMany()
                        .HasForeignKey("IncomesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Model.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
