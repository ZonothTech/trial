using TrailShop.Domain.Entities;

namespace TrailShop.Application.Abstractions;

public interface IOrderRepository
{
    /// <summary>
    /// Loads an order with its lines and line products.
    /// </summary>
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds all orders placed by the customer with the given email address.
    /// </summary>
    Task<IReadOnlyList<Order>> FindByCustomerEmailAsync(string email, CancellationToken cancellationToken = default);
}
