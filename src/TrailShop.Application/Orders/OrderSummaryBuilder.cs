using TrailShop.Domain.Entities;

namespace TrailShop.Application.Orders;

/// <summary>
/// Builds the plain-text order summary used in confirmation emails.
/// Legacy code: written before PricingCalculator existed.
/// </summary>
public class OrderSummaryBuilder
{
    public string BuildPlainTextSummary(Order order, Customer customer)
    {
        string s = "";
        s = s + "TrailShop order #" + order.Id + "\n";
        s = s + "Customer: " + customer.DisplayName + "\n";
        s = s + "----\n";
        decimal sub = 0;
        int n = 0;
        for (int i = 0; i < order.Lines.Count; i++)
        {
            var l = order.Lines[i];
            var name = l.Product != null ? l.Product.Name : "Item " + l.ProductId;
            s = s + l.Quantity + " x " + name + " @ " + l.UnitPrice.ToString("0.00") + " = " + (l.Quantity * l.UnitPrice).ToString("0.00") + "\n";
            sub = sub + l.Quantity * l.UnitPrice;
            n = n + l.Quantity;
        }
        s = s + "----\n";
        s = s + "Subtotal: " + sub.ToString("0.00") + "\n";
        decimal d1 = 0;
        if (n > 100)
        {
            d1 = Math.Round(sub * 0.05m, 2, MidpointRounding.AwayFromZero);
            s = s + "Bulk discount: -" + d1.ToString("0.00") + "\n";
        }
        decimal d2 = 0;
        if (customer.LoyaltyTier != LoyaltyTier.None)
        {
            if (customer.LoyaltyTier == LoyaltyTier.Gold)
            {
                d2 = Math.Round((sub - d1) * 0.10m, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                if (customer.LoyaltyTier == LoyaltyTier.Silver)
                {
                    d2 = Math.Round((sub - d1) * 0.03m, 2, MidpointRounding.AwayFromZero);
                }
            }
            if (d2 > 0)
            {
                s = s + "Loyalty discount (" + customer.LoyaltyTier + "): -" + d2.ToString("0.00") + "\n";
            }
        }
        s = s + "Total: " + (sub - d1 - d2).ToString("0.00") + "\n";
        s = s + "Ship to: " + order.ShippingAddress + "\n";
        return s;
    }
}
