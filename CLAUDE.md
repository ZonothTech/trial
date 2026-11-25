# TrailShop: project instructions for Claude Code

TrailShop is a deliberately vulnerable ASP.NET Core training app (.NET 10, EF Core + SQLite, xUnit).
It runs only locally in Development. Never suggest deploying it.

## Commands
- Build: `dotnet build`
- Test: `dotnet test` (run it after every code change; a change is not done until tests pass)
- Format check: `dotnet format --verify-no-changes`
- Run: `dotnet run --project src/TrailShop.Api` (http://localhost:5080)
- Vulnerable packages: `dotnet list package --vulnerable --include-transitive`

## Layout and dependency rules (see docs/architecture/target-architecture.md)
- Domain -> nothing. Application -> Domain. Infrastructure -> Application, Domain. Api -> Application (+ Infrastructure only in Program.cs).
- Controllers and Razor pages call Application services. Never inject `ShopDbContext` into Api code.
- Public endpoints return DTOs, never EF entities.

## Conventions
- Async all the way; async methods end in `Async` and take a `CancellationToken`.
- Constructor injection only.
- Money is `decimal`, rounded with `MidpointRounding.AwayFromZero`. Format numbers with `CultureInfo.InvariantCulture`.
- Business rules for prices live in docs/pricing-rules.md. If code and that file disagree, ask before changing either.
- Tests: xUnit, one behaviour per test, descriptive snake_case names. Integration tests use `ShopApiFactory`.

## Security rules (non-negotiable)
- Never build SQL from strings. Use LINQ, `FromSqlInterpolated`/`FromSql`, or explicit parameters.
- Every controller or action declares `[Authorize]` (with a policy or role where needed) or `[AllowAnonymous]` with a one-line reason.
- Check resource ownership in the Application layer. Return 404, not 403, for resources the caller may not see.
- Never log headers, tokens, passwords, connection strings, emails, addresses, or coupon codes. Log IDs instead.
- Never put secrets in code or appsettings*.json. Use `dotnet user-secrets` locally.
- Do not use `Html.Raw` on data a user or admin can write.
- Validate request input at the API boundary (length, format, range) and return ProblemDetails on failure.

## Working agreement
- For changes touching more than two files, or anything in auth, pricing, coupons, or data access: propose a plan first and wait for approval.
- Do not edit files under `.github/`, `.claude/`, or `tools/` unless asked.
- Do not run `git push`, `git reset --hard`, `git clean`, or history-rewriting commands. Suggest them instead.
