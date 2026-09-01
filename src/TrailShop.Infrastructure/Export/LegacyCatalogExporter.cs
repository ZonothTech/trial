using Newtonsoft.Json;
using TrailShop.Domain.Entities;

namespace TrailShop.Infrastructure.Export;

/// <summary>
/// Catalog export/import used by the reporting team. Predates System.Text.Json adoption.
/// </summary>
public class LegacyCatalogExporter
{
    public string Export(IEnumerable<Product> products)
    {
        var rows = products.Select(p => new ProductImportRow
        {
            Sku = p.Sku,
            Name = p.Name,
            UnitPrice = p.UnitPrice
        });

        return JsonConvert.SerializeObject(rows, Formatting.Indented);
    }

    public IReadOnlyList<ProductImportRow> ParseImport(string json)
    {
        return JsonConvert.DeserializeObject<List<ProductImportRow>>(json) ?? new List<ProductImportRow>();
    }
}

public class ProductImportRow
{
    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
}
