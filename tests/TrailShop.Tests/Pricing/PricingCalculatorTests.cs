using TrailShop.Domain.Entities;
using TrailShop.Domain.Pricing;

namespace TrailShop.Tests.Pricing;

public class PricingCalculatorTests
{
    private readonly PricingCalculator _calculator = new();

    private static OrderLine Line(int quantity, decimal unitPrice) => new() { Quantity = quantity, UnitPrice = unitPrice };

    private static Customer CustomerWith(LoyaltyTier tier) => new() { Id = 99, Username = "test", LoyaltyTier = tier };

    [Fact]
    public void Subtotal_is_the_sum_of_line_totals()
    {
        var quote = _calculator.CalculateQuote(new[] { Line(2, 10.00m), Line(1, 5.50m) }, CustomerWith(LoyaltyTier.None));

        Assert.Equal(25.50m, quote.Subtotal);
        Assert.Equal(25.50m, quote.Total);
    }

    [Fact]
    public void Gold_customer_gets_ten_percent_loyalty_discount()
    {
        var quote = _calculator.CalculateQuote(new[] { Line(1, 200.00m) }, CustomerWith(LoyaltyTier.Gold));

        Assert.Equal(20.00m, quote.LoyaltyDiscount);
        Assert.Equal(180.00m, quote.Total);
    }

    [Fact]
    public void Silver_customer_gets_three_percent_loyalty_discount()
    {
        var quote = _calculator.CalculateQuote(new[] { Line(1, 200.00m) }, CustomerWith(LoyaltyTier.Silver));

        Assert.Equal(6.00m, quote.LoyaltyDiscount);
        Assert.Equal(194.00m, quote.Total);
    }

    [Fact]
    public void Bulk_discount_applies_to_large_orders()
    {
        var quote = _calculator.CalculateQuote(new[] { Line(150, 2.00m) }, CustomerWith(LoyaltyTier.None));

        Assert.Equal(15.00m, quote.BulkDiscount);
        Assert.Equal(285.00m, quote.Total);
    }

    [Fact]
    public void Rejects_non_positive_quantity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _calculator.CalculateQuote(new[] { Line(0, 10.00m) }, CustomerWith(LoyaltyTier.None)));
    }
}
