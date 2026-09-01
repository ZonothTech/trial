using TrailShop.Domain.Entities;

namespace TrailShop.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> ListListedAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> SearchAsync(string term, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
