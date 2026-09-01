# PROP-007: Extract pricing into a separate microservice

Author: (fictional) platform enthusiast
Status: Proposed, not decided

## Proposal

Move `PricingCalculator` and all discount logic into a new **Pricing Service**:

- separate ASP.NET Core service with its own repository and CI pipeline
- its own database for discount rules
- gRPC API called synchronously by the shop for every quote and every order summary
- deployed to its own container with independent scaling

## Claimed benefits

- "Pricing can scale independently."
- "The pricing team can deploy without coordinating."
- "It is the modern architecture; monoliths do not scale."

## Facts the proposal does not mention

- TrailShop has one team of five engineers. There is no separate pricing team.
- Pricing is pure in-memory arithmetic over an order that is already loaded.
- Peak traffic is about 40 quotes per minute.
- The upcoming roadmap item is coupon redemption, which needs pricing and order data in the same transaction.
