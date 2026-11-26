using Microsoft.EntityFrameworkCore;
using TrailShop.Application.Abstractions;
using TrailShop.Domain.Entities;
using TrailShop.Infrastructure.Data;

namespace TrailShop.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ShopDbContext _db;

    public OrderRepository(ShopDbContext db)
    {
        _db = db;
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
                .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> FindByCustomerEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT o.* FROM Orders o JOIN Customers c ON c.Id = o.CustomerId WHERE c.Email = '" + email + "'";

        return await _db.Orders
            .FromSqlRaw(sql)
            .AsNoTracking()
            .Include(o => o.Lines)
            .ToListAsync(cancellationToken);
    }
}
