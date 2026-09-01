namespace TrailShop.Domain.Pricing;

public sealed record PriceQuote(
    decimal Subtotal,
    decimal BulkDiscount,
    decimal LoyaltyDiscount,
    decimal Total);
