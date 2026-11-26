using TrailShop.Domain.Entities;

namespace TrailShop.Domain.Pricing;

/// <summary>
/// Calculates order prices. The business rules live in docs/pricing-rules.md.
/// </summary>
public class PricingCalculator
{
    /// <summary>
    /// Minimum total units, summed across all lines, for an order to get the bulk discount.
    /// An order of exactly this many units qualifies.
    /// </summary>
    public const int BulkDiscountThresholdUnits = 100;

    /// <summary>
    /// Bulk discount as a fraction of the subtotal (5%).
    /// </summary>
    public const decimal BulkDiscountRate = 0.05m;

    /// <summary>
    /// Loyalty discount for <see cref="LoyaltyTier.Silver"/> customers (3%),
    /// applied to the amount after the bulk discount.
    /// </summary>
    public const decimal SilverRate = 0.03m;

    /// <summary>
    /// Loyalty discount for <see cref="LoyaltyTier.Gold"/> customers (10%),
    /// applied to the amount after the bulk discount.
    /// </summary>
    public const decimal GoldRate = 0.10m;

    /// <summary>
    /// Calculates the subtotal, discounts and total for an order.
    /// </summary>
    /// <remarks>
    /// <list type="number">
    /// <item><description>Subtotal is the sum of unit price times quantity for each line. The unit price is
    /// the one captured on the order line, not the current catalog price.</description></item>
    /// <item><description>Orders of <see cref="BulkDiscountThresholdUnits"/> units or more get a bulk discount
    /// of <see cref="BulkDiscountRate"/> of the subtotal.</description></item>
    /// <item><description>The loyalty discount is applied to the amount after the bulk discount:
    /// <see cref="LoyaltyTier.None"/> 0%, <see cref="LoyaltyTier.Silver"/> <see cref="SilverRate"/>,
    /// <see cref="LoyaltyTier.Gold"/> <see cref="GoldRate"/>.</description></item>
    /// <item><description>Each discount is rounded to 2 decimal places, midpoint away from zero, before it
    /// is subtracted.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="lines">The order lines to price.</param>
    /// <param name="customer">The customer who owns the order; their loyalty tier sets the loyalty discount.
    /// Must not be <see langword="null"/>.</param>
    /// <returns>The subtotal, bulk discount, loyalty discount and total.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lines"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">A line has a quantity less than 1, or a negative
    /// unit price.</exception>
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

        var bulkDiscount = units >= BulkDiscountThresholdUnits
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
