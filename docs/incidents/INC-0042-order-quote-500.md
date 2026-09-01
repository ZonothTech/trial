# INC-0042: Quote endpoint returns HTTP 500 for some orders

Severity: Medium (fictional training incident)
Reported by: Customer support
Affected endpoint: `GET /api/orders/{id}/quote`

## Report

Customer **bob** (customer id 2) opened the quote for his draft order **5** and got an HTTP 500 error. Other customers have not complained. Support reproduced it locally with:

```http
GET http://localhost:5080/api/orders/5/quote
X-Demo-User: bob
```

## Log excerpt

Captured from the starter build of TrailShop; file paths shortened to repository-relative paths and framework frames trimmed.

```text
info: TrailShop.Application.Orders.OrderService[0]
      Calculating quote for order 5 for customer (null)
fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
      An unhandled exception has occurred while executing the request.
      System.NullReferenceException: Object reference not set to an instance of an object.
         at TrailShop.Domain.Pricing.PricingCalculator.ApplyLoyaltyDiscount(Decimal amount, Customer customer) in src/TrailShop.Domain/Pricing/PricingCalculator.cs:line 49
         at TrailShop.Domain.Pricing.PricingCalculator.CalculateQuote(IReadOnlyCollection`1 lines, Customer customer) in src/TrailShop.Domain/Pricing/PricingCalculator.cs:line 42
         at TrailShop.Application.Orders.OrderService.GetQuoteAsync(Int32 orderId, CancellationToken cancellationToken) in src/TrailShop.Application/Orders/OrderService.cs:line 48
         at TrailShop.Api.Controllers.OrdersController.GetQuote(Int32 id, CancellationToken cancellationToken) in src/TrailShop.Api/Controllers/OrdersController.cs:line 29
         ...
```

## Ask

Find the root cause, fix it, and add a test that reproduces the failure. Then check whether any other orders are affected.
