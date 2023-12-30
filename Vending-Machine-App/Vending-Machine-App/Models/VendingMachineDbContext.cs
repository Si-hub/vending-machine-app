using Microsoft.EntityFrameworkCore;

namespace Vending_Machine_App.Models;

/// <summary>
/// Represents the database context for the vending machine application.
/// </summary>
public partial class VendingMachineDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VendingMachineDbContext"/> class.
    /// </summary>
    public VendingMachineDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VendingMachineDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options for configuring the database context.</param>
    public VendingMachineDbContext(DbContextOptions<VendingMachineDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of items in the vending machine.
    /// </summary>
    public virtual DbSet<Item> Items { get; set; }

    /// <summary>
    /// Gets or sets the collection of purchases made in the vending machine.
    /// </summary>
    public virtual DbSet<Purchase> Purchases { get; set; }

    /// <summary>
    /// Configures the options for the database context.
    /// </summary>
    /// <param name="optionsBuilder">The builder used to configure the options.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__items__52020FDD8B472EAF");

            entity.ToTable("items");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("item_name");
            entity.Property(e => e.ItemPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("item_price");
            entity.Property(e => e.ItemQuantity)
                .HasColumnType("int")
                .HasColumnName("item_quantity");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__purchase__87071CB95534AE18");

            entity.ToTable("purchases");

            entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");
            entity.Property(e => e.AmountPaid)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount_paid");
            entity.Property(e => e.Change)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("change");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("item_name");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("datetime")
                .HasColumnName("purchase_date");

            entity.HasOne(d => d.Item).WithMany()
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_purchases_items");
        });

        SeedDefaultItems(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    private static void SeedDefaultItems(ModelBuilder modelBuilder)
    {

        var items = new[] { 
            new Item {ItemId = 1,ItemName = "Sprite",ItemPrice = 20.00m,ItemQuantity = 10 },
            new Item {ItemId = 2,ItemName = "Coke",ItemPrice = 5.00m,ItemQuantity = 10 },
            new Item {ItemId = 3,ItemName = "Water",ItemPrice = 20.00m,ItemQuantity = 10 },
            new Item {ItemId = 4,ItemName = "Oreo",ItemPrice = 5.00m,ItemQuantity = 1 },
            new Item {ItemId = 5, ItemName = "Chips", ItemPrice = 20.00m,ItemQuantity = 10 },
            new Item {ItemId = 7, ItemName = "Pepsi", ItemPrice = 10.00m,ItemQuantity = 10 },
            new Item {ItemId = 6, ItemName = "Twist", ItemPrice = 20.00m,ItemQuantity = 10 },
            new Item {ItemId = 8, ItemName = "Stoney", ItemPrice = 10.00m,ItemQuantity = 10 },
            new Item {ItemId = 9, ItemName = "BarOne", ItemPrice = 5.00m,ItemQuantity = 10 }

        };
        modelBuilder.Entity<Item>().HasData(items);
    }

}


