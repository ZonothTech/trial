using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TrailShop.Domain.Entities;

/// <summary>
/// A product in the TrailShop catalog.
/// </summary>
[Index(nameof(Sku), IsUnique = true)]
public class Product
{
    public int Id { get; set; }

    [MaxLength(32)]
    public string Sku { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// HTML fragment written by store admins and shown on the product page.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// What TrailShop pays the supplier. Internal business data.
    /// </summary>
    [Precision(18, 2)]
    public decimal CostPrice { get; set; }

    /// <summary>
    /// Unlisted products are hidden from the public catalog (prototypes, withdrawn items).
    /// </summary>
    public bool IsListed { get; set; } = true;
}
