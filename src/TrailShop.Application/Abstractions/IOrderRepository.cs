using TrailShop.Domain.Entities;

namespace TrailShop.Application.Abstractions;

public interface IOrderRepository
{
    /// <summary>
    /// Loads an order with its lines and line products.
    /// </summary>
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
