using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrailShop.Application.Catalog;
using TrailShop.Domain.Entities;

namespace TrailShop.Api.Pages.Products;

public class DetailsModel : PageModel
{
    private readonly CatalogService _catalog;

    public DetailsModel(CatalogService catalog)
    {
        _catalog = catalog;
    }

    public Product Product { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _catalog.GetProductAsync(id, cancellationToken);
        if (product is null || !product.IsListed)
        {
            return NotFound();
        }

        Product = product;
        return Page();
    }
}
