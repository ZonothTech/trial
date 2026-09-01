using TrailShop.Domain.Entities;

namespace TrailShop.Domain.Pricing;

/// <summary>
/// Calculates order prices. The business rules live in docs/pricing-rules.md.
/// </summary>
public class PricingCalculator
{
    public const int BulkDiscountThresholdUnits = 100;
    public const decimal BulkDiscountRate = 0.05m;
    public const decimal SilverRate = 0.03m;
    public const decimal GoldRate = 0.10m;

    public PriceQuote CalculateQuote(IReadOnlyCollection<OrderLine> lines, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(lines);

        decimal subtotal = 0m;
        int units = 0;

        foreach (var line in lines)
        {
            if (line.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lines), "Quantity must be positive.");
            }

            if (line.UnitPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lines), "Unit price cannot be negative.");
            }

            subtotal += line.UnitPrice * line.Quantity;
            units += line.Quantity;
        }

        var bulkDiscount = units > BulkDiscountThresholdUnits
            ? Round(subtotal * BulkDiscountRate)
            : 0m;

        var loyaltyDiscount = ApplyLoyaltyDiscount(subtotal - bulkDiscount, customer);

        return new PriceQuote(subtotal, bulkDiscount, loyaltyDiscount, subtotal - bulkDiscount - loyaltyDiscount);
    }

    private static decimal ApplyLoyaltyDiscount(decimal amount, Customer customer)
    {
        var rate = customer.LoyaltyTier switch
        {
            LoyaltyTier.Gold => GoldRate,
            LoyaltyTier.Silver => SilverRate,
            _ => 0m
        };

        return Round(amount * rate);
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
