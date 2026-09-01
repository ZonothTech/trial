using System.Net;
using System.Net.Http.Json;
using TrailShop.Application.Orders;

namespace TrailShop.Tests.Api;

public class OrdersEndpointTests : IClassFixture<ShopApiFactory>
{
    private readonly ShopApiFactory _factory;

    public OrdersEndpointTests(ShopApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_order_without_demo_user_returns_401()
    {
        var client = _factory.CreateClientAs(null);

        var response = await client.GetAsync("/api/orders/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Customer_can_read_own_order()
    {
        var client = _factory.CreateClientAs("alice");

        var order = await client.GetFromJsonAsync<OrderDto>("/api/orders/1");

        Assert.NotNull(order);
        Assert.Equal(1, order!.CustomerId);
        Assert.Equal(2, order.Lines.Count);
    }

    [Fact]
    public async Task Quote_for_gold_customer_order_applies_loyalty_discount()
    {
        var client = _factory.CreateClientAs("alice");

        var quote = await client.GetFromJsonAsync<PriceQuoteDto>("/api/orders/1/quote");

        Assert.NotNull(quote);
        Assert.Equal(358.90m, quote!.Subtotal);
        Assert.Equal(0m, quote.BulkDiscount);
        Assert.Equal(35.89m, quote.LoyaltyDiscount);
        Assert.Equal(323.01m, quote.Total);
    }
}
