using System.ComponentModel.DataAnnotations;

namespace Vending_Machine_App.Models;

/// <summary>
/// Represents an item in the vending machine.
/// </summary>
public partial class Item
{
    /// <summary>
    /// Gets or sets the ID of the item.
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    [Required(ErrorMessage = "Item name is required.")]
    public string? ItemName { get; set; }

    /// <summary>
    /// Gets or sets the price of the item.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal? ItemPrice { get; set; }

    public int? ItemQuantity { get; set; }

    //public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
