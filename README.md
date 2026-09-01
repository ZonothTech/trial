# TrailShop (training app)

> **Vulnerable by design. Local training use only.**
> TrailShop contains deliberate security defects for the course *Claude Code for Secure .NET Development (L100/L200)*. Never deploy it, never expose it on a network, and never copy its code into a real product.
> The app refuses to start unless `ASPNETCORE_ENVIRONMENT=Development`.

TrailShop is a small, fictional outdoor-gear shop: a product catalog, customer orders, and price quotes with bulk and loyalty discounts. You will use the same repository in all ten sessions of the course.

## Safety notes

- Every credential-like value in this repository is **synthetic**. It was generated for training and has never been valid anywhere. Host names use the reserved `.invalid` top-level domain and email addresses use `.example`, so nothing can connect to a real system.
- Do not paste real secrets, customer data, or code from your employer into this repository or into prompts while working on it.
- Treat everything Claude Code proposes here as you would in a real repository: read the diff, run the tests, then decide.

## Prerequisites

- .NET SDK 10.0.100 or later (`dotnet --version`). The repository's `global.json` accepts any 10.0 feature band.
- Git 2.40 or later.
- Visual Studio 2026 with the **ASP.NET and web development** workload (optional for command-line work).

## First-time setup

The course ships this folder without Git history. First copy it out of the course's student repository into a folder of its own (for example `C:\src\TrailShop`); the setup script refuses to run inside another Git repository. Then create the training history once (it adds three commits, the tag `course-start`, and a local bare repository next to this folder that acts as `origin`):

```powershell
# Windows PowerShell or PowerShell 7
powershell -ExecutionPolicy Bypass -File .\tools\Initialize-TrainingRepo.ps1
```

```bash
# macOS, Linux, or Git Bash
bash tools/initialize-training-repo.sh
```

On Windows, the course's setup script (`setup\Initialize-WindowsMachine.ps1` in the student repository) does the copy and this step for you, together with the tool installs.

If your instructor gave you a GitHub copy of the repository instead, clone it and skip this step.

## Build, test, run

```powershell
dotnet build
dotnet test
dotnet run --project src/TrailShop.Api
```

The API listens on `http://localhost:5080`. Open `src/TrailShop.Api/TrailShop.Api.http` in Visual Studio to send sample requests. The SQLite database file `trailshop.db` is created and seeded on first run; delete it to reset the data.

**Expected warning:** restore and build report `NU1903` for one package. Leave it alone until the course tells you otherwise.

## Demo users

TrailShop uses a **training stand-in for authentication**: the `X-Demo-User` request header names the user you are acting as. Real systems use OpenID Connect or JWT bearer tokens; this shortcut exists only so the course can focus on other topics.

| Username | Role | Loyalty tier |
|----------|------|--------------|
| `alice`  | Customer | Gold |
| `bob`    | Customer | Silver |
| `carol`  | Customer | None |
| `admin`  | Store admin | None |

## Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/products` | List products in the public catalog |
| GET | `/api/products/search?q=` | Search products by name |
| GET | `/api/products/{id}` | Get one product |
| GET | `/api/products/by-sku/{sku}` | Get a product by SKU |
| GET | `/api/orders/{id}` | Get an order |
| GET | `/api/orders/{id}/quote` | Price quote for an order |
| PUT | `/api/admin/products/{id}` | Update price and description |
| GET | `/api/admin/catalog/export` | Export the catalog as JSON |
| POST | `/api/admin/catalog/import-preview` | Parse a catalog import file and preview it |
| GET | `/products/{id}` | Product page (Razor) |

## Solution layout

```
TrailShop.sln
src/
  TrailShop.Domain/          Entities and the pricing calculator
  TrailShop.Application/     Use-case services and repository interfaces
  TrailShop.Infrastructure/  EF Core (SQLite), repositories, integrations
  TrailShop.Api/             ASP.NET Core controllers, Razor page, composition root
tests/
  TrailShop.Tests/           xUnit unit and integration tests
docs/                        Pricing rules, target architecture, incidents, proposals
exercises/                   Patches the course applies at specific points
tools/                       Setup scripts and a local review script (Session 6 fallback)
.claude/settings.json        Project setting: start Claude Code sessions in Manual mode
```

## Why `.claude/settings.json` sets Manual mode

Recent Claude Code versions can start in *auto* mode, where a classifier approves many actions for you. In this course you approve every edit and command yourself, so you see what the agent does and practise reviewing it. The project setting `"defaultMode": "default"` makes terminal sessions start in Manual mode. You can still change mode during a session with `Shift+Tab`.
