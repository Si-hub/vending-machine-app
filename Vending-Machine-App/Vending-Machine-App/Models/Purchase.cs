
using System.ComponentModel.DataAnnotations;

namespace Vending_Machine_App.Models;

/// <summary>
/// Represents a purchase made in the vending machine.
/// </summary>
public partial class Purchase
{


    /// <summary>
    /// Gets or sets the unique identifier of the purchase.
    /// </summary>
    public int PurchaseId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the item purchased.
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the item purchased.
    /// </summary>
    [Required(ErrorMessage = "Please select an item.")]
    public string ItemName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the amount paid for the purchase.
    /// </summary>
    [Required(ErrorMessage = "Please enter the amount.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal AmountPaid { get; set; }

    /// <summary>
    /// Gets or sets the change given for the purchase.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Change must be greater than or equal to 0.")]
    public decimal? Change { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the purchase.
    /// </summary>
    [Required(ErrorMessage = "Purchase date is required.")]
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// Gets or sets the item associated with the purchase.
    /// </summary>
    public virtual Item? Item { get; set; }
}
