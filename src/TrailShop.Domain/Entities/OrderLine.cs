using Microsoft.EntityFrameworkCore;

namespace TrailShop.Domain.Entities;

public class OrderLine
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit captured when the line was added to the order.
    /// </summary>
    [Precision(18, 2)]
    public decimal UnitPrice { get; set; }
}
