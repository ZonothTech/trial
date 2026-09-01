using TrailShop.Application.Abstractions;
using TrailShop.Domain.Entities;

namespace TrailShop.Application.Catalog;

public class CatalogService
{
    private readonly IProductRepository _products;

    public CatalogService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<IReadOnlyList<ProductSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var products = await _products.ListListedAsync(cancellationToken);
        return products.Select(ToSummary).ToList();
    }

    public async Task<IReadOnlyList<ProductSummaryDto>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        var products = await _products.SearchAsync(term, cancellationToken);
        return products.Select(ToSummary).ToList();
    }

    public Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken = default)
    {
        return _products.GetByIdAsync(id, cancellationToken);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return _products.GetBySkuAsync(sku, cancellationToken);
    }

    private static ProductSummaryDto ToSummary(Product p) => new(p.Id, p.Sku, p.Name, p.UnitPrice);
}
