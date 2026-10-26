# SmartiePaws -- Architecture Overview

This doc describes the overall shape of the system: how it's structured today, and the
target structure it's being deliberately built toward. See `database-design.md` for the
current MVP's table-level schema.

## Scope

SmartiePaws is growing beyond a single inventory tool into a small platform:

- **Inventory** -- products, variants, vendors, stock (FIFO costing).
- **Point of Sale** -- ringing up sales, consuming inventory.
- **Scheduling / Booking** -- appointments for dog training, dog sitting, and dog watching.
- **Billing** -- invoicing and payment, backed by Stripe rather than homegrown payment handling.
- **Identity** -- user accounts and roles (customers, staff/admin), replacing the earlier
  "anonymous guest" MVP assumption now that billing and bookings require knowing who's who.
- **AI** -- a customer-facing chatbot (product Q&A, helping customers purchase) and an
  admin-facing bot (helping staff manage inventory and interact with the system).

## Current approach: modular monolith

Rather than building separate services from day one, the system is one solution following
Clean Architecture (`SharedKernel -> Domain -> Application -> Infrastructure -> Web.Api`),
with each bounded context above organized as its own top-level module instead of one flat
feature list:

```
Domain/
  Identity/        users, roles
  Catalog/         Product, ProductVariant, Vendor
  Sales/           PurchaseOrder, Sale, SaleLineItem, FIFO lot consumption
  Scheduling/      Service type, Appointment, Availability
  Billing/         Invoice, Stripe customer/subscription refs
  AI/              conversation/audit logs for agent activity
Application/
  Identity/{UseCase}/
  Catalog/{UseCase}/
  Sales/{UseCase}/
  Scheduling/{UseCase}/
  Billing/{UseCase}/
  AI/{UseCase}/
```

Each use case is a CQRS command or query handler (no MediatR): a typed input, FluentValidation,
a handler returning `Result`/`Result<T>`, no exceptions for expected failures. One database,
but each module's tables live in their own Postgres schema so there are no cross-module joins --
module boundaries are real even though everything runs in one process today.

**Why a monolith first:** this is a solo-built system. Microservices add real operational cost
(network calls instead of function calls, eventual consistency, N deployments instead of 1,
service-to-service auth) that isn't worth paying until something concrete forces it. The module
boundaries above are chosen specifically so each one can be cut out into its own service later
with minimal rework -- see "Migration path" below.

**Why CQRS handlers matter beyond code organization:** each command/query handler is already
shaped like an LLM tool-call (typed input -> validation -> a `Result`, nothing leaking from other
layers). Both AI bots are built by exposing a curated set of existing handlers as tools, not by
building a second, parallel "AI-safe" API.

## Target structure: microservices (future)

```
                              +-----------------------+
                              |  API Gateway / BFF     |  routes requests, validates JWT,
                              |                        |  rate limits, aggregates responses
                              +-----------+-----------+
                                          |
     +-----------+--------------+---------+---------+--------------+---------------+
     |           |              |         |         |              |               |
+----+----+ +----+----+   +-----+-----+ +-+--------+ +-----+------+ +------+------+
|Identity | |Catalog  |   |Inventory/ | |Scheduling| |Billing     | |AI / Agent   |
|Service  | |Service  |   |Sales (POS)| |/Booking  | |Service     | |Service      |
+---------+ +---------+   |Service    | |Service   | +------------+ +-------------+
                           +-----------+ +----------+
     (each box: own database, own deploy, same Clean Architecture shape internally)
                                        |
                    event bus (RabbitMQ / Kafka / SQS)
       SaleCompleted . AppointmentBooked . InvoicePaid . ProductUpdated . InventoryAdjusted
```

### What each service owns and does

- **API Gateway / BFF** -- the single entry point for web/mobile/POS clients. Validates the
  caller's JWT, routes to the right service, rate-limits, and aggregates responses when a screen
  needs data from more than one service. Owns no business data itself.

- **Identity Service** -- user accounts, roles/permissions (customer vs. staff vs. admin), login
  and token issuance. Every other service trusts the same JWT rather than calling Identity on
  every request.

- **Catalog Service** -- the product catalog: `Product`, `ProductVariant`, `Vendor`. Read-heavy;
  this is what the customer chatbot queries to answer "do you have X" / "how much is Y."

- **Inventory / Sales (POS) Service** -- restocking (`PurchaseOrder`), ringing up sales (`Sale`,
  `SaleLineItem`), and FIFO cost/lot tracking. Owns the actual stock levels. The admin AI bot's
  inventory actions (adjust stock, review low-stock items) go through this service's existing
  commands.

- **Scheduling / Booking Service** -- appointment types (training / sitting / watching), booking,
  and staff availability. Not yet designed at the entity level -- needs its own design pass
  before being built, the way `database-design.md` was done for inventory.

- **Billing Service** -- invoices and payment status, backed by Stripe (customer, subscription,
  and invoice references plus webhook handling). Deliberately thin: no raw card data, no
  homegrown payment processing. Listens for `SaleCompleted` and `AppointmentBooked` events to know
  when to generate an invoice, rather than being called synchronously by Sales or Scheduling.

- **AI / Agent Service** -- the customer chatbot (product Q&A, helping customers buy) and the
  admin bot (helping staff manage inventory / interact with the system). Owns almost no business
  data of its own (just conversation history / audit logs of what it did) -- it acts by calling
  the other services' APIs as tools, under either the end user's own identity or a dedicated
  service identity (to be decided before this is built).

### How services connect

- **Synchronous (REST/gRPC)** for request/response needs: the gateway calling any service, or the
  AI service calling Catalog/Sales/Scheduling to answer a question or take an action on a user's
  behalf.
- **Asynchronous (events)** for facts other services react to without being called directly:
  Billing reacts to `SaleCompleted` / `AppointmentBooked`; nothing needs to call Billing directly
  to trigger an invoice.
- **No cross-service database joins, ever.** Each service owns its data exclusively; anything
  another service needs is fetched via API or kept as a local, event-updated copy (e.g. Sales
  caching a product's name/price from a `ProductUpdated` event rather than joining into Catalog's
  database).
- **Auth propagates, not re-authenticates.** Every service validates the same JWT (or a service
  account token for machine-to-machine calls like the AI service hitting Sales) instead of calling
  Identity on every request.

### Migration path: module -> service

When a specific module needs to become its own service (a concrete scaling, team, or compliance
reason -- not just "it feels tidier"):

1. Move that module's `Domain/{Module}`, `Application/{Module}`, and related `Infrastructure`
   code into its own solution.
2. Give it its own `Web.Api` host and its own Postgres database (it already has its own schema,
   so there's no data to untangle).
3. Replace any in-process calls to it with HTTP calls or events; add an outbox so publishing an
   event is transactionally safe with that service's own database write.
4. Point the gateway at the new service instead of the monolith.

Because the module boundaries in the monolith already match these future service boundaries, this
is a cut-and-wire exercise, not a redesign.

## Open design areas (not yet settled)

- **Scheduling/Booking** has no entity-level design yet -- appointment types, availability, and
  staff assignment all need a design pass before any code is written.
- **AI service identity** -- whether each bot acts under the end user's own token or a dedicated
  service account needs to be decided before the first agent tool is wired up.
- **Billing** -- confirm Stripe (or chosen provider) before building the `Billing` module, since
  its schema is mostly a mirror of whatever that provider's objects look like.
