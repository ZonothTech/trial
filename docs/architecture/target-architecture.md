# TrailShop target architecture

Status: agreed by the team (fictional), September 2026.

TrailShop is a **modular monolith**: one deployable ASP.NET Core application, one database, clear internal layers. We split services only when a concrete scaling, ownership, or reliability need justifies the operational cost.

## Layers and allowed dependencies

```
TrailShop.Api            -> TrailShop.Application, TrailShop.Infrastructure (composition root only)
TrailShop.Infrastructure -> TrailShop.Application, TrailShop.Domain
TrailShop.Application    -> TrailShop.Domain
TrailShop.Domain         -> (nothing: no EF Core, no ASP.NET Core, no third-party frameworks)
```

## Rules

1. **Domain is persistence-ignorant.** Entities and domain services do not reference EF Core or any data-access attribute. Mapping lives in `TrailShop.Infrastructure` (Fluent API).
2. **Controllers and Razor pages call Application services only.** They never use `ShopDbContext` or repositories directly. `TrailShop.Api` references Infrastructure only to register services in `Program.cs`.
3. **Application defines the ports** (repository and gateway interfaces); Infrastructure implements them.
4. **API responses use DTOs**, never EF entities.
5. **Authorization is explicit.** Every controller or action declares `[Authorize]` (with a role or policy where needed) or `[AllowAnonymous]`.

## Out of scope today

- Separate services or databases per module.
- Message brokers.
