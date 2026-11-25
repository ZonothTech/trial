using System.Net;
using System.Net.Http.Json;
using TrailShop.Application.Orders;
using TrailShop.Tests.Api;

namespace TrailShop.Tests.Orders;

/// <summary>
/// Regression tests for INC-0042. Expected figures come from docs/pricing-rules.md
/// applied to the seeded orders.
/// </summary>
public class QuoteRegressionTests : IClassFixture<ShopApiFactory>
{
    private readonly ShopApiFactory _factory;

    public QuoteRegressionTests(ShopApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Quote_for_bob_order_5_returns_200_with_silver_discount()
    {
        var client = _factory.CreateClientAs("bob");

        var response = await client.GetAsync("/api/orders/5/quote");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var quote = await response.Content.ReadFromJsonAsync<PriceQuoteDto>();
        Assert.NotNull(quote);
        Assert.Equal(179.00m, quote!.Subtotal);
        Assert.Equal(5.37m, quote.LoyaltyDiscount);
        Assert.Equal(173.63m, quote.Total);
    }

    [Fact]
    public async Task Quote_for_carol_order_2_applies_no_loyalty_discount()
    {
        var client = _factory.CreateClientAs("carol");

        var quote = await client.GetFromJsonAsync<PriceQuoteDto>("/api/orders/2/quote");

        Assert.NotNull(quote);
        Assert.Equal(0.00m, quote!.LoyaltyDiscount);
        Assert.Equal(149.90m, quote.Total);
    }

    [Fact]
    public async Task Quote_for_alice_order_4_applies_gold_discount_of_order_owner()
    {
        var client = _factory.CreateClientAs("alice");

        var quote = await client.GetFromJsonAsync<PriceQuoteDto>("/api/orders/4/quote");

        Assert.NotNull(quote);
        Assert.Equal(9.88m, quote!.LoyaltyDiscount);
        Assert.Equal(88.92m, quote.Total);
    }

    [Fact]
    public async Task Quote_for_bob_order_3_with_exactly_100_units_applies_bulk_and_silver_discounts()
    {
        var client = _factory.CreateClientAs("bob");

        var quote = await client.GetFromJsonAsync<PriceQuoteDto>("/api/orders/3/quote");

        Assert.NotNull(quote);
        Assert.Equal(72.50m, quote!.BulkDiscount);
        Assert.Equal(41.33m, quote.LoyaltyDiscount);
        Assert.Equal(1336.17m, quote.Total);
    }
}
