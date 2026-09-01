using Microsoft.EntityFrameworkCore;
using TrailShop.Application.Abstractions;
using TrailShop.Domain.Entities;
using TrailShop.Infrastructure.Data;

namespace TrailShop.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ShopDbContext _db;

    public CustomerRepository(ShopDbContext db)
    {
        _db = db;
    }

    public Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return _db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Username == username, cancellationToken);
    }
}
