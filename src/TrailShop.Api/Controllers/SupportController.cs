using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrailShop.Application.Orders;

namespace TrailShop.Api.Controllers;

/// <summary>
/// Lets support staff look up a customer's orders during a phone call.
/// </summary>
[ApiController]
[Authorize]
[Route("api/support")]
public class SupportController : ControllerBase
{
    private readonly OrderService _orders;

    public SupportController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> FindOrders([FromQuery] string email, CancellationToken cancellationToken)
    {
        return Ok(await _orders.FindByCustomerEmailAsync(email, cancellationToken));
    }
}
