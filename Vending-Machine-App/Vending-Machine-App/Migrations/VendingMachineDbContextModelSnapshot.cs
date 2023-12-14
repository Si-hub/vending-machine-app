﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vending_Machine_App.Models;

#nullable disable

namespace Vending_Machine_App.Migrations
{
    [DbContext(typeof(VendingMachineDbContext))]
    partial class VendingMachineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Vending_Machine_App.Models.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("item_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemId"));

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("item_name");

                    b.Property<decimal?>("ItemPrice")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("item_price");

                    b.HasKey("ItemId")
                        .HasName("PK__items__52020FDD8B472EAF");

                    b.ToTable("items", (string)null);

                    b.HasData(
                        new
                        {
                            ItemId = 1,
                            ItemName = "Item 1",
                            ItemPrice = 2.09m
                        },
                        new
                        {
                            ItemId = 2,
                            ItemName = "Item 2",
                            ItemPrice = 2.19m
                        },
                        new
                        {
                            ItemId = 3,
                            ItemName = "Item 3",
                            ItemPrice = 2.29m
                        },
                        new
                        {
                            ItemId = 4,
                            ItemName = "Item 4",
                            ItemPrice = 2.39m
                        },
                        new
                        {
                            ItemId = 5,
                            ItemName = "Item 5",
                            ItemPrice = 2.49m
                        },
                        new
                        {
                            ItemId = 6,
                            ItemName = "Item 6",
                            ItemPrice = 2.59m
                        },
                        new
                        {
                            ItemId = 7,
                            ItemName = "Item 7",
                            ItemPrice = 2.69m
                        },
                        new
                        {
                            ItemId = 8,
                            ItemName = "Item 8",
                            ItemPrice = 2.79m
                        },
                        new
                        {
                            ItemId = 9,
                            ItemName = "Item 9",
                            ItemPrice = 2.89m
                        },
                        new
                        {
                            ItemId = 10,
                            ItemName = "Item 10",
                            ItemPrice = 2.99m
                        },
                        new
                        {
                            ItemId = 11,
                            ItemName = "Item 11",
                            ItemPrice = 3.09m
                        },
                        new
                        {
                            ItemId = 12,
                            ItemName = "Item 12",
                            ItemPrice = 3.19m
                        },
                        new
                        {
                            ItemId = 13,
                            ItemName = "Item 13",
                            ItemPrice = 3.29m
                        },
                        new
                        {
                            ItemId = 14,
                            ItemName = "Item 14",
                            ItemPrice = 3.39m
                        },
                        new
                        {
                            ItemId = 15,
                            ItemName = "Item 15",
                            ItemPrice = 3.49m
                        },
                        new
                        {
                            ItemId = 16,
                            ItemName = "Item 16",
                            ItemPrice = 3.59m
                        },
                        new
                        {
                            ItemId = 17,
                            ItemName = "Item 17",
                            ItemPrice = 3.69m
                        },
                        new
                        {
                            ItemId = 18,
                            ItemName = "Item 18",
                            ItemPrice = 3.79m
                        },
                        new
                        {
                            ItemId = 19,
                            ItemName = "Item 19",
                            ItemPrice = 3.89m
                        },
                        new
                        {
                            ItemId = 20,
                            ItemName = "Item 20",
                            ItemPrice = 3.99m
                        });
                });

            modelBuilder.Entity("Vending_Machine_App.Models.Purchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("purchase_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"));

                    b.Property<decimal>("AmountPaid")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("amount_paid");

                    b.Property<decimal?>("Change")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("change");

                    b.Property<int>("ItemId")
                        .HasColumnType("int")
                        .HasColumnName("item_id");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("item_name");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime")
                        .HasColumnName("purchase_date");

                    b.HasKey("PurchaseId")
                        .HasName("PK__purchase__87071CB95534AE18");

                    b.HasIndex("ItemId");

                    b.ToTable("purchases", (string)null);
                });

            modelBuilder.Entity("Vending_Machine_App.Models.Purchase", b =>
                {
                    b.HasOne("Vending_Machine_App.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .IsRequired()
                        .HasConstraintName("FK_purchases_items");

                    b.Navigation("Item");
                });
#pragma warning restore 612, 618
        }
    }
}
