# TrailShop pricing rules

Owner: Commercial team. Last reviewed: September 2026 (fictional).

These rules are the specification for `PricingCalculator`. If code and this document disagree, this document wins and the code has a bug.

## 1. Subtotal

Subtotal = sum of (line unit price x line quantity). The unit price is the price captured on the order line, not the current catalog price.

## 2. Bulk discount

An order of **100 units or more** in total (summed across all lines) gets a bulk discount of **5% of the subtotal**.

## 3. Loyalty discount

Applied to the amount after the bulk discount.

| Tier   | Discount |
|--------|----------|
| None   | 0%       |
| Silver | 3%       |
| Gold   | 10%      |

The tier is the tier of **the customer who owns the order**.

## 4. Rounding

Each discount is rounded to 2 decimal places, midpoint away from zero, before it is subtracted.

## 5. Validation

- Quantity must be at least 1.
- Unit price cannot be negative.

## 6. Reserved for future work

Coupons and promotional codes are not supported yet. See the coupon-redemption proposal when it exists.
