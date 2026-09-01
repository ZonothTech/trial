using Microsoft.EntityFrameworkCore;
using TrailShop.Application.Abstractions;
using TrailShop.Domain.Entities;
using TrailShop.Infrastructure.Data;

namespace TrailShop.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _db;

    public ProductRepository(ShopDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> ListListedAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => p.IsListed)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Products WHERE IsListed = 1 AND Name LIKE '%" + term + "%' ORDER BY Name";

        return await _db.Products
            .FromSqlRaw(sql)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return _db.Products
            .FromSqlInterpolated($"SELECT * FROM Products WHERE Sku = {sku}")
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
