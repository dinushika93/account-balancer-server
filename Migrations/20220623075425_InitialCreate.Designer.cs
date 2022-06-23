﻿// <auto-generated />
using AccountBalancer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AccountBalancer.Migrations
{
    [DbContext(typeof(AccountBalanceContext))]
    [Migration("20220623075425_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AccountBalancer.Models.AccountBalance", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("CanteenBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("CarBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MarketingBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ParkingFinesBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("RDBalance")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("AccountBalances");
                });
#pragma warning restore 612, 618
        }
    }
}
