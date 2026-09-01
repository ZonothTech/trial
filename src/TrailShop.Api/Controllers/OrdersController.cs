using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrailShop.Application.Orders;

namespace TrailShop.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;

    public OrdersController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> Get(int id, CancellationToken cancellationToken)
    {
        var order = await _orders.GetOrderAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("{id:int}/quote")]
    public async Task<ActionResult<PriceQuoteDto>> GetQuote(int id, CancellationToken cancellationToken)
    {
        var quote = await _orders.GetQuoteAsync(id, cancellationToken);
        return quote is null ? NotFound() : Ok(quote);
    }
}
