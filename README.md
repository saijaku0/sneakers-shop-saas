# Sneakers Shop

An educational, full-stack e-commerce platform for a sneaker store. The point of the
project isn't the domain - it's the engineering underneath it: a **.NET backend** built
around Domain-Driven Design, CQRS and real concurrency control, paired with a **Next.js
frontend** organized with Feature-Sliced Design.

It is deliberately **not** production software. The goal is to demonstrate architectural
depth on a realistic vertical slice rather than to ship a store.

---

## What it does

A single, fully-working vertical flow, covered by tests end to end:

**registration -> catalog with filters -> product page -> cart -> checkout with a concurrent
inventory reservation -> order details / order list.**

The scope is intentionally narrow. Discounts, wishlists, comments and admin tooling exist
in the domain model but are deferred to later iterations - the MVP validates the core
entities and the hardest part of the flow (checkout under contention), not breadth.

---

## Tech stack

**Backend**
- .NET 10 / C#
- Clean Architecture (Domain / Application / Infrastructure / API)
- Domain-Driven Design - rich aggregates, invariants enforced inside boundaries
- CQRS with MediatR; pipeline behaviors for validation and concurrency retry
- EF Core over PostgreSQL
- FluentValidation
- ASP.NET Core Identity for authentication

**Frontend** (`web/`)
- Next.js 16 (App Router) + React 19 + TypeScript
- Feature-Sliced Design
- Redux Toolkit + RTK Query for server and client state
- React Hook Form + Zod for forms and validation
- Radix UI + shadcn/ui + Tailwind CSS v4
- Embla (carousels), Sonner (toasts), next-themes

**Tooling**
- pnpm workspace (monorepo)
- Docker for local infrastructure (`infra/`)
- Postman collection (`.postman/`) for the API

---

## Repository layout

```
.
├── backend/        .NET solution - Domain / Application / Infrastructure / API + tests
├── web/            Next.js frontend (Feature-Sliced Design)
├── infra/          local infrastructure (containers)
├── .postman/       API collection
├── .github/        CI workflows
└── pnpm-workspace.yaml
```

---

## Architecture highlights

### Domain model

Aggregates are drawn around invariants and lifecycle, not around database tables:

- **Product** - the showcase (model, description, base price, images). Knows nothing about
  stock, so products can be listed even when out of stock. A root because inventory,
  comments, wishlist items and discounts all reference it.
- **WarehouseItem** - inventory. No `Warehouse` wrapper, because there is no invariant that
  spans rows; stock levels are independent per row. **Invariant:** `reserved <= quantity`,
  enforced on a single row and protected by a concurrency token. Carries an embedded
  **Size value object** (`sizeSystem`, `sizeValue`) instead of a Size table.
- **Cart** - the user's mutable, abandonable cart. Separate from Order by design: different
  lifecycle, different pricing rules. Prices are **live** (computed on read from
  `Product.basePrice` + active discounts). Adding to the cart does **not** reserve stock -
  reservation happens only at checkout, so one item sitting in N carts can't lock inventory.
- **Order** - a placed order. **Invariant:** `totalAmount` equals the sum of items, and
  prices are **frozen as a snapshot** (`unitPrice`) at checkout - the opposite of the cart.
  Progresses through a status state machine (Pending -> Paid -> …).
- **Brand**, **UserProfile** - small independent roots.

`CartItem` and `OrderItem` are entities *inside* their aggregates and mutate only through
the root.

### Checkout & concurrency - the core of the project

Checkout is an application-level operation that coordinates three aggregates by ID (never by
nesting): read the cart, call `WarehouseItem.Reserve(quantity)` per line (which validates
`reserved <= quantity` under a concurrency token), freeze prices into `OrderItem`, create the
`Order` as `Pending`, and clear the cart - all in one transaction.

The interesting part is step two. A **`ConcurrencyRetryBehavior`** in the MediatR pipeline,
combined with the row concurrency token, guarantees **no oversell** under parallel checkouts.
This is backed by a regression test: **N parallel checkouts competing for the last unit ->
exactly one succeeds.**

### Read/write separation

- **Read side** goes through `IApplicationDbContext` - projects straight into DTOs with
  `AsNoTracking`.
- **Write side** goes through repositories per aggregate, committed via a shared
  `CommandHandler` base and `IUnitOfWork`.

Controllers return `Result` mapped to HTTP via a single `MapError` (ProblemDetails +
`errorCode`), rather than wrapping in `Ok()`.

### Authentication

ASP.NET Core Identity - self-hosted, no external provider to stand up. Registration, login,
password hashing and sessions come from the framework. `UserProfile.Id` maps to the Identity
user id, and `[Authorize]` handlers read the user id from token claims via
`ICurrentUserService`.

---

## Testing

- **Domain unit tests** for aggregate invariants and state transitions.
- **Integration tests** through a custom `WebApplicationFactory` over real HTTP, with a
  `TestAuthHelper` issuing tokens.
- **Concurrency regression test** - the checkout no-oversell scenario described above.

---

## Getting started

> Commands below are the intended shape of the workflow; adjust to your local setup.

**Prerequisites:** .NET 10 SDK, Node.js + pnpm, Docker.

```bash
# 1. Start local infrastructure (database, etc.)
docker compose -f infra/docker-compose.yml up -d

# 2. Backend - from the backend/ solution
dotnet run --project backend/src/API

# 3. Frontend
pnpm install
pnpm --filter web dev
```

---

## Roadmap

- **Domain event dispatch** - the mechanism to raise events (`OrderPaid`,
  `WarehouseItemReserved`, `OrderCancelled`) is in place; the first real subscriber is
  `OrderCancelled -> release inventory`. Dispatch ships together with that first reaction, not
  as an empty pipe.
- Order cancellation by the user
- Profile page (name / email / password via Identity, not raw updates)
- Product descriptions, discounts (`Discount` + Sale filter)
- Admin area, wishlist, comments with moderation
- Redis caching for the catalog, email confirmation
- End-to-end tests (full-flow xUnit chain on the backend, Playwright on checkout)
