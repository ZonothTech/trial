using System.ComponentModel.DataAnnotations;

namespace TrailShop.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Draft;

    public DateTime CreatedUtc { get; set; }

    [MaxLength(512)]
    public string ShippingAddress { get; set; } = string.Empty;

    public List<OrderLine> Lines { get; set; } = new();
}
