using TrailShop.Domain.Entities;

namespace TrailShop.Application.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Customer?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
