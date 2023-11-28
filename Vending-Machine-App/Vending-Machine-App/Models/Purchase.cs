using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vending_Machine_App.Models;

public partial class Purchase
{
    

    public int PurchaseId { get; set; }

    public int ItemId { get; set; }

    [Required(ErrorMessage = "Please select an item.")]
    public string ItemName { get; set; } = null!;

    [Required(ErrorMessage = "Please enter the amount.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal AmountPaid { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Change must be greater than or equal to 0.")]
    public decimal? Change { get; set; }

    [Required(ErrorMessage = "Purchase date is required.")]
    public DateTime PurchaseDate { get; set; }

    public virtual Item? Item { get; set; }
}
