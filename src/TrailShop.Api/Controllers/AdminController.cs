using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrailShop.Infrastructure.Data;
using TrailShop.Infrastructure.Export;

namespace TrailShop.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ShopDbContext _db;
    private readonly LegacyCatalogExporter _exporter;

    public AdminController(ShopDbContext db, LegacyCatalogExporter exporter)
    {
        _db = db;
        _exporter = exporter;
    }

    public sealed record UpdateProductRequest(decimal UnitPrice, string Description);

    [HttpPut("products/{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        product.UnitPrice = request.UnitPrice;
        product.Description = request.Description;
        await _db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("catalog/export")]
    public async Task<IActionResult> ExportCatalog(CancellationToken cancellationToken)
    {
        var products = await _db.Products.AsNoTracking().ToListAsync(cancellationToken);
        return Content(_exporter.Export(products), "application/json");
    }

    [HttpPost("catalog/import-preview")]
    public async Task<IActionResult> PreviewImport(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync(cancellationToken);
        var rows = _exporter.ParseImport(json);
        return Ok(new { rowCount = rows.Count, skus = rows.Select(r => r.Sku) });
    }
}
