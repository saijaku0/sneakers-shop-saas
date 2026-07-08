# Sneakers Shop - Backend

### What is an MVP and what is it built around

**MVP** (Minimum Viable Product) is the minimum version of a product with a set of functionality
sufficient to validate the core business entities and domain model.
The scope intentionally does not include all entities in the schema: the MVP slice is the chain
“catalog -> cart -> inventory reservation -> order -> payment”. Discount, Comment, Wishlist
are deferred to subsequent iterations.

### Authentication

**ASP.NET Core Identity** is used - a self-hosted solution built into ASP.NET Core:
logins and password hashes are stored in its own database. The choice is motivated by the fact
that there is no need to deploy and configure an external provider (Auth0); registration, login,
hashing, and session management are provided “out of the box” and are tightly integrated with
the ASP.NET Core pipeline.
`UserProfile.id` corresponds to the user identifier from Identity.

### Entities and aggregates

- **Brand** - an *aggregate* (a small independent root). Stores the brand name.
  Product references Brand by `brandId`; the relationship is one-to-many: one brand = many
  products. It is a separate root because Brand is referenced from multiple places
  (Product, Discount): a shared reference cannot be someone else's child object.

- **Product** - an *aggregate*, the product showcase (model, description, base price, images).
  Product knows nothing about inventory - WarehouseItem references Product by ID.
  It is intentionally separated so that products can be displayed even when they are currently
  out of stock (preparation for a future availability date). It is a root because it is referenced by
  WarehouseItem, Comment, WishlistItem, Discount.

- **WarehouseItem** - an *aggregate* (inventory). There is no `Warehouse` wrapper: it would not
  enforce any invariant across positions - stock levels of different rows are independent.
  **Invariant:** `reserved <= quantity`, enforced within the boundary of a single row. Concurrent
  access is protected by a concurrency token (`xmin`/version) on the same row. Contains an
  **embedded Size VO** (`sizeSystem`, `sizeValue`) - there is no separate Size table;
  the VO is responsible for representing sizes in EU/US systems.

- **Cart** - an *aggregate*, the user's mutable shopping cart. A separate root, **NOT part of
  Order**: the cart has a different lifecycle (draft, can be abandoned) and a different relationship
  to pricing. Key decisions:

  - Prices in the cart are **live** - `CartItem` does not store the price; it is calculated on read
    from `Product.basePrice` + active discounts. (Compare with `OrderItem`, which stores a
    price snapshot.)
  - Adding an item to the cart does **NOT reserve inventory**. Reservation
    (`WarehouseItem.Reserve()`) happens only during checkout - otherwise a single item in N
    carts would lock all available stock.
  - One active cart per user (`userId` unique).
    **Invariants:** `quantity > 0` for an item; no duplicate SKUs within a cart (adding the same
    SKU again increases the quantity).

- **CartItem** - an *entity* within the Cart aggregate (not a root). References a specific
  `WarehouseItem` (product + size = SKU) by ID. Mutates only through the Cart root
  (`internal` mutation methods).

- **Order** - an *aggregate*, a placed order. Created at checkout and then progresses through
  statuses (Pending -> Paid -> …). **Invariant:** `totalAmount` = sum of all items;
  prices are **frozen as a snapshot** (`unitPrice`) at the time of checkout.

- **OrderItem** - an *entity* within the Order aggregate (not a root). An order line containing
  a price snapshot. It has no external references and lives and dies together with Order.

- **UserProfile** - an *aggregate*. Holds WishlistItem as child entities.

### Checkout

An Application-level operation that links three aggregates (by ID, not by nesting):

1. Read the user's `Cart`.
2. For each item - call `WarehouseItem.Reserve(quantity)` (validation of `reserved <= quantity`,
   concurrency token on the inventory row).
3. Freeze prices as snapshots in `OrderItem` (`unitPrice`, `discountAmount`).
4. Create an `Order` with status `Pending`.
5. Clear the `Cart`.

The concurrency logic lives specifically in step 2 (regression test: N parallel checkouts for the
last pair of items -> exactly 1 success).
