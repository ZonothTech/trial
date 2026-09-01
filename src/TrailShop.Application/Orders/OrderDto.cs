namespace TrailShop.Application.Orders;

public sealed record OrderDto(
    int Id,
    int CustomerId,
    string Status,
    DateTime CreatedUtc,
    string ShippingAddress,
    IReadOnlyList<OrderLineDto> Lines);

public sealed record OrderLineDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice);

public sealed record PriceQuoteDto(
    int OrderId,
    decimal Subtotal,
    decimal BulkDiscount,
    decimal LoyaltyDiscount,
    decimal Total);
