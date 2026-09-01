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
}
