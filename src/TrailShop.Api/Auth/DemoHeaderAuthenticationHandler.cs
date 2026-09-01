using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TrailShop.Application.Abstractions;

namespace TrailShop.Api.Auth;

/// <summary>
/// TRAINING STAND-IN FOR REAL AUTHENTICATION.
/// Trusts the X-Demo-User header and signs the caller in as that seeded user.
/// Anyone can send any header value, so this is only acceptable because the
/// app refuses to run outside a local Development environment (see Program.cs).
/// A real system would use OpenID Connect / JWT bearer tokens.
/// </summary>
public class DemoHeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "DemoHeader";
    public const string HeaderName = "X-Demo-User";
    public const string AdminRole = "Admin";

    private readonly ICustomerRepository _customers;

    public DemoHeaderAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ICustomerRepository customers)
        : base(options, logger, encoder)
    {
        _customers = customers;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var values) || string.IsNullOrWhiteSpace(values))
        {
            return AuthenticateResult.NoResult();
        }

        var customer = await _customers.GetByUsernameAsync(values.ToString(), Context.RequestAborted);
        if (customer is null)
        {
            return AuthenticateResult.Fail("Unknown demo user.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new(ClaimTypes.Name, customer.Username)
        };

        if (customer.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, AdminRole));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName));
    }
}
