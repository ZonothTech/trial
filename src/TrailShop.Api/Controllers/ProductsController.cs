using Microsoft.AspNetCore.Mvc;
using TrailShop.Application.Catalog;

namespace TrailShop.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly CatalogService _catalog;

    public ProductsController(CatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductSummaryDto>>> List(CancellationToken cancellationToken)
    {
        return Ok(await _catalog.ListAsync(cancellationToken));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<ProductSummaryDto>>> Search([FromQuery] string q, CancellationToken cancellationToken)
    {
        return Ok(await _catalog.SearchAsync(q, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var product = await _catalog.GetProductAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("by-sku/{sku}")]
    public async Task<IActionResult> GetBySku(string sku, CancellationToken cancellationToken)
    {
        var product = await _catalog.GetBySkuAsync(sku, cancellationToken);
        return product is null || !product.IsListed
            ? NotFound()
            : Ok(new ProductSummaryDto(product.Id, product.Sku, product.Name, product.UnitPrice));
    }
}
