using System.Net;
using System.Net.Http.Json;
using TrailShop.Application.Catalog;

namespace TrailShop.Tests.Api;

public class ProductsEndpointTests : IClassFixture<ShopApiFactory>
{
    private readonly ShopApiFactory _factory;

    public ProductsEndpointTests(ShopApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task List_returns_only_listed_products()
    {
        var client = _factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<ProductSummaryDto>>("/api/products");

        Assert.NotNull(products);
        Assert.Equal(7, products!.Count);
        Assert.DoesNotContain(products, p => p.Sku == "TS-PROTO-X");
    }

    [Fact]
    public async Task Search_finds_products_by_name()
    {
        var client = _factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<ProductSummaryDto>>("/api/products/search?q=Tent");

        Assert.NotNull(products);
        var match = Assert.Single(products!);
        Assert.Equal("TS-TENT-2P", match.Sku);
    }

    [Fact]
    public async Task Get_by_sku_returns_listed_product()
    {
        var client = _factory.CreateClient();

        var product = await client.GetFromJsonAsync<ProductSummaryDto>("/api/products/by-sku/TS-BOOT-M42");

        Assert.NotNull(product);
        Assert.Equal("Granite Hiking Boot", product!.Name);
    }

    [Fact]
    public async Task Get_by_unknown_sku_returns_404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products/by-sku/NOPE-123");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
