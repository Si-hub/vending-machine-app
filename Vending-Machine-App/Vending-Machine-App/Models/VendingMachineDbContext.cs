using Microsoft.EntityFrameworkCore;

namespace Vending_Machine_App.Models;

public partial class VendingMachineDbContext : DbContext
{
    public VendingMachineDbContext()
    {
    }

    public VendingMachineDbContext(DbContextOptions<VendingMachineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<Purchase> Purchases { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Remove any specific configuration here as it will be handled by DI
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.ToTable("items");

            entity.Property(e => e.ItemId)
                .HasColumnName("item_id");

            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");

            entity.Property(e => e.ItemPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("item_price");

            entity.Property(e => e.ItemQuantity)
                .HasColumnName("item_quantity");

            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasColumnName("category");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId);

            entity.ToTable("purchases");

            entity.Property(e => e.PurchaseId)
                .HasColumnName("purchase_id");

            entity.Property(e => e.AmountPaid)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount_paid");

            entity.Property(e => e.Change)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("change");

            entity.Property(e => e.ItemId)
                .HasColumnName("item_id");

            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");

            entity.Property(e => e.PurchaseDate)
                .HasColumnType("timestamp with time zone")  // PostgreSQL timestamp
                .HasColumnName("purchase_date");

            entity.HasOne(d => d.Item)
                .WithMany()
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_purchases_items");
        });

        // Seed data for all database types
        SeedDefaultItems(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    private static void SeedDefaultItems(ModelBuilder modelBuilder)
    {
        var items = new[]
        {
            new Item { ItemId = 1, ItemName = "Sprite", ItemPrice = 20.00m, ItemQuantity = 10, Category = "Drink" },
            new Item { ItemId = 2, ItemName = "Coke", ItemPrice = 5.00m, ItemQuantity = 10, Category = "Drink" },
            new Item { ItemId = 3, ItemName = "Water", ItemPrice = 20.00m, ItemQuantity = 10, Category = "Drink" },
            new Item { ItemId = 4, ItemName = "Oreo", ItemPrice = 5.00m, ItemQuantity = 1, Category = "Snack" },
            new Item { ItemId = 5, ItemName = "Chips", ItemPrice = 20.00m, ItemQuantity = 10, Category = "Snack" },
            new Item { ItemId = 7, ItemName = "Pepsi", ItemPrice = 10.00m, ItemQuantity = 10, Category = "Drink" },
            new Item { ItemId = 6, ItemName = "Twist", ItemPrice = 20.00m, ItemQuantity = 10, Category = "Drink" },
            new Item { ItemId = 8, ItemName = "Stoney", ItemPrice = 10.00m, ItemQuantity = 10, Category = "Drink" },
            new Item { ItemId = 9, ItemName = "BarOne", ItemPrice = 5.00m, ItemQuantity = 10, Category = "Snack" }
        };
        modelBuilder.Entity<Item>().HasData(items);
    }
}