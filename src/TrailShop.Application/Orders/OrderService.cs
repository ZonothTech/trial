using Microsoft.Extensions.Logging;
using TrailShop.Application.Abstractions;
using TrailShop.Domain.Entities;
using TrailShop.Domain.Pricing;

namespace TrailShop.Application.Orders;

public class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICustomerRepository _customers;
    private readonly PricingCalculator _pricing;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orders,
        ICustomerRepository customers,
        PricingCalculator pricing,
        ILogger<OrderService> logger)
    {
        _orders = orders;
        _customers = customers;
        _pricing = pricing;
        _logger = logger;
    }

    public async Task<OrderDto?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken);
        return order is null ? null : ToDto(order);
    }

    public async Task<PriceQuoteDto?> GetQuoteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        var customer = await _customers.GetByIdAsync(order.CustomerId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"Order {order.Id} references customer {order.CustomerId}, which does not exist.");

        _logger.LogInformation(
            "Calculating quote for order {OrderId} for customer {CustomerId}",
            order.Id,
            customer.Id);

        var quote = _pricing.CalculateQuote(order.Lines, customer);

        return new PriceQuoteDto(order.Id, quote.Subtotal, quote.BulkDiscount, quote.LoyaltyDiscount, quote.Total);
    }

    public async Task<IReadOnlyList<OrderDto>> FindByCustomerEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Support lookup for customer {CustomerEmail}", email);

        var orders = await _orders.FindByCustomerEmailAsync(email, cancellationToken);
        return orders.Select(ToDto).ToList();
    }

    private static OrderDto ToDto(Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status.ToString(),
        order.CreatedUtc,
        order.ShippingAddress,
        order.Lines
            .Select(l => new OrderLineDto(l.ProductId, l.Product?.Name ?? string.Empty, l.Quantity, l.UnitPrice))
            .ToList());
}
