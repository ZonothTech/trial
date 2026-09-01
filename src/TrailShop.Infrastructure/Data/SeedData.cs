using TrailShop.Domain.Entities;

namespace TrailShop.Infrastructure.Data;

/// <summary>
/// Synthetic training data. Every name, email and address is fictional.
/// Email domains use the reserved .example TLD (RFC 2606).
/// </summary>
public static class SeedData
{
    public static void EnsureSeeded(ShopDbContext db)
    {
        if (db.Products.Any())
        {
            return;
        }

        db.Products.AddRange(
            new Product { Id = 1, Sku = "TS-TENT-2P", Name = "Ridgeline 2P Tent", UnitPrice = 289.00m, CostPrice = 171.40m, Description = "<p>Freestanding two-person tent. <strong>2.1 kg</strong> packed.</p>" },
            new Product { Id = 2, Sku = "TS-BOOT-M42", Name = "Granite Hiking Boot", UnitPrice = 149.90m, CostPrice = 88.00m, Description = "<p>Waterproof leather boot with a stiff midsole.</p>" },
            new Product { Id = 3, Sku = "TS-SOCK-MW", Name = "Merino Trail Socks", UnitPrice = 14.50m, CostPrice = 4.10m, Description = "<p>Midweight merino blend. Sold per pair.</p>" },
            new Product { Id = 4, Sku = "TS-STOVE-1", Name = "Ember Camp Stove", UnitPrice = 59.00m, CostPrice = 27.75m, Description = "<p>Canister stove, boils 1 L in about 4 minutes.</p>" },
            new Product { Id = 5, Sku = "TS-HEAD-300", Name = "Lumen 300 Headlamp", UnitPrice = 34.95m, CostPrice = 12.20m, Description = "<p>300 lumen headlamp with red night mode.</p>" },
            new Product { Id = 6, Sku = "TS-PACK-45", Name = "Summit 45L Backpack", UnitPrice = 179.00m, CostPrice = 96.30m, Description = "<p>45 litre pack with adjustable back system.</p>" },
            new Product { Id = 7, Sku = "TS-PROTO-X", Name = "Prototype Ultralight Shelter (staff only)", UnitPrice = 999.00m, CostPrice = 640.00m, IsListed = false, Description = "<p>Unreleased prototype. Not for sale.</p>" },
            new Product { Id = 8, Sku = "TS-BOTTLE-1L", Name = "Canyon Water Bottle 1L", UnitPrice = 19.90m, CostPrice = 5.60m, Description = "<p>Insulated steel bottle.</p>" });

        db.Customers.AddRange(
            new Customer { Id = 1, Username = "alice", DisplayName = "Alice Example", Email = "alice@trailshop.example", LoyaltyTier = LoyaltyTier.Gold },
            new Customer { Id = 2, Username = "bob", DisplayName = "Bob Example", Email = "bob@trailshop.example", LoyaltyTier = LoyaltyTier.Silver },
            new Customer { Id = 3, Username = "carol", DisplayName = "Carol Example", Email = "carol@trailshop.example", LoyaltyTier = LoyaltyTier.None },
            new Customer { Id = 4, Username = "admin", DisplayName = "Store Admin", Email = "admin@trailshop.example", LoyaltyTier = LoyaltyTier.None, IsAdmin = true });

        var created = new DateTime(2026, 9, 1, 9, 0, 0, DateTimeKind.Utc);

        db.Orders.AddRange(
            NewOrder(1, customerId: 1, OrderStatus.Placed, created, "1 Sample Street, Exampleton", (1, 1, 289.00m), (5, 2, 34.95m)),
            NewOrder(2, customerId: 3, OrderStatus.Placed, created.AddDays(1), "3 Test Lane, Mockford", (2, 1, 149.90m)),
            NewOrder(3, customerId: 2, OrderStatus.Placed, created.AddDays(2), "2 Demo Avenue, Placeholder City", (3, 100, 14.50m)),
            NewOrder(4, customerId: 1, OrderStatus.Placed, created.AddDays(3), "1 Sample Street, Exampleton", (4, 1, 59.00m), (8, 2, 19.90m)),
            NewOrder(5, customerId: 2, OrderStatus.Draft, created.AddDays(4), "2 Demo Avenue, Placeholder City", (6, 1, 179.00m)),
            NewOrder(6, customerId: 3, OrderStatus.Draft, created.AddDays(5), "3 Test Lane, Mockford", (1, 1, 289.00m), (3, 4, 14.50m)),
            NewOrder(7, customerId: 1, OrderStatus.Draft, created.AddDays(6), "1 Sample Street, Exampleton", (2, 1, 149.90m)));

        db.SaveChanges();
    }

    private static Order NewOrder(
        int id,
        int customerId,
        OrderStatus status,
        DateTime createdUtc,
        string shippingAddress,
        params (int ProductId, int Quantity, decimal UnitPrice)[] lines)
    {
        return new Order
        {
            Id = id,
            CustomerId = customerId,
            Status = status,
            CreatedUtc = createdUtc,
            ShippingAddress = shippingAddress,
            Lines = lines
                .Select(l => new OrderLine { ProductId = l.ProductId, Quantity = l.Quantity, UnitPrice = l.UnitPrice })
                .ToList()
        };
    }
}
