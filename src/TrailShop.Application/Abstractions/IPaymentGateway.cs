namespace TrailShop.Application.Abstractions;

public interface IPaymentGateway
{
    Task<PaymentAuthorization> AuthorizeAsync(int orderId, decimal amount, CancellationToken cancellationToken = default);
}

public sealed record PaymentAuthorization(bool Approved, string Reference);
