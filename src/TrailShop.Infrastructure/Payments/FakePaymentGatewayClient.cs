using System.Security.Cryptography;
using System.Text;
using TrailShop.Application.Abstractions;

namespace TrailShop.Infrastructure.Payments;

/// <summary>
/// Offline stand-in for a payment provider. It never makes a network call:
/// it signs the request locally and always approves.
/// </summary>
public class FakePaymentGatewayClient : IPaymentGateway
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    private const string ApiKey = "pgw_live_6q2Zr8Vt1Lx4Nc7Hb3Mj9Kd5Pw0Sy";

    public Task<PaymentAuthorization> AuthorizeAsync(int orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var payload = $"{ApiKeyHeaderName}|{orderId}|{amount:0.00}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
        var signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

        return Task.FromResult(new PaymentAuthorization(true, "SIM-" + signature[..12]));
    }
}
