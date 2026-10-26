# SmartiePaws — notes for Claude Code

Working conventions for this repo, built from the Clean Architecture template
(https://www.milanjovanovic.tech). Match them so generated code looks hand-written.
See `docs/README.md` for the overall system/architecture plan (modular monolith now,
target microservices split later) and `docs/database-design.md` for the current schema.

## Architecture (dependencies point inward)

- **SharedKernel** — `Result`/`Result<T>`, `Error`/`ErrorType`, `ValidationError`, `Entity`, `IDomainEvent`, `IDomainEventHandler<T>`, `IDateTimeProvider`. No dependencies.
- **Domain** — entities, domain events, error catalogs. References SharedKernel only. No EF, no application types.
- **Application** — use cases as vertical slices, grouped by bounded-context module, then use case: `{Module}/{UseCase}/` — e.g. `Identity/Login/`, `Catalog/CreateProduct/`, `Sales/RingUpSale/`. Current modules: `Identity` (kept from the template's Users/auth sample), with `Catalog`, `Sales`, `Scheduling`, `Billing`, `AI` added as each is designed and built (see `docs/README.md`). MediatR-free `ICommand`/`IQuery` + handlers, FluentValidation validators, validation + logging decorators (Scrutor), abstractions (`IApplicationDbContext`, `IEmailSender`, auth). References Domain + SharedKernel.
- **Infrastructure** — EF Core + PostgreSQL (snake_case), migrations, JWT auth + refresh tokens, permission authorization, HybridCache, domain-event dispatch, `LoggingEmailSender`. References Application.
- **Web.Api** — minimal-API endpoints (`IEndpoint`, auto-discovered), Scalar/OpenAPI, rate limiting, security headers, CORS, idempotency filter, global exception handling + `ProblemDetails`, OpenTelemetry. References Infrastructure.

## Conventions

- **CQRS, no MediatR** — `ICommand`/`ICommand<T>` + `ICommandHandler<...>`, `IQuery<T>` + `IQueryHandler<Q,R>`. Handlers are `internal sealed` with primary constructors, auto-registered by Scrutor scanning; wrapped by `ValidationDecorator` + `LoggingDecorator`.
- **Errors, not exceptions** — return `Result.Failure(...)` with an `Error` for expected failures. Error codes are `"{FeaturePlural}.{Reason}"`. Endpoints translate with `result.Match(Results.Ok, CustomResults.Problem)`.
- **Endpoints** implement `IEndpoint`; protect with `.RequireAuthorization()` / `.HasPermission("users:access")`; opt into dedupe with `.WithIdempotency()` (honors an `Idempotency-Key` header).
- **Validation** — FluentValidation `AbstractValidator<TCommand>`; runs in the validation decorator.
- **Data** — inject `IApplicationDbContext`; entities use FK ids (shadow relationships), no navigations. Configurations in `Infrastructure/{Feature}/` are auto-applied.
- **List queries** return `PagedList<T>` (search + sort + page).
- **Caching** — HybridCache with per-feature cache-key helpers + invalidation.
- **Observability** — Serilog + OpenTelemetry export over OTLP to the Grafana/Loki/Tempo stack (`docker compose up -d`; Grafana at http://localhost:3000).

## Build & test

- `dotnet build SmartiePaws.slnx`
- `dotnet test SmartiePaws.slnx` — integration tests use Testcontainers, so Docker must be running.
- **Analyzer-strict**: `TreatWarningsAsErrors` + SonarAnalyzer. Keep code warning-clean; prefer collection expressions (`[.. x]`) and `ToUpperInvariant`.

## Skills (prefer these for common tasks)

`/add-feature` (use case) · `/add-entity` · `/add-tests` · `/add-migration` · `/add-background-job` · `/bootstrap` (rename + strip the sample feature) · `/ca-review` (convention review).
