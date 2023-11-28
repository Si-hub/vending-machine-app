using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vending_Machine_App.Models;

public partial class Item
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item name is required.")]
    public string? ItemName { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal? ItemPrice { get; set; }

    //public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
