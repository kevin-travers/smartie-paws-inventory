# SmartiePaws

Inventory, point-of-sale, scheduling, billing, and AI-assisted workflows for a dog
training/sitting/watching business. Built as a Clean Architecture modular monolith —
see `docs/README.md` for the architecture plan and `docs/database-design.md` for the schema.

## Stack

- **SharedKernel** — `Result`/`Error`, `Entity`, domain event abstractions.
- **Domain** — entities, domain events, error catalogs, grouped by module (`Identity`, and more
  as they're designed).
- **Application** — use cases as vertical slices (MediatR-free `ICommand`/`IQuery` + handlers),
  FluentValidation, validation + logging decorators.
- **Infrastructure** — EF Core + PostgreSQL (snake_case naming, migrations), JWT auth + refresh
  tokens, permission-based authorization, HybridCache, Serilog.
- **Web.Api** — minimal API endpoints, rate limiting, OpenTelemetry tracing/metrics, global
  exception handling + `ProblemDetails`, Scalar/OpenAPI.
- **Grafana + Loki + Tempo + Prometheus** via the bundled `grafana/otel-lgtm` image — Grafana at
  http://localhost:3000.
- **Testing** — `ArchitectureTests` (layer boundaries), `Application.UnitTests`,
  `IntegrationTests` (Testcontainers).

## Getting started

```bash
docker compose up -d        # PostgreSQL + Grafana/Loki/Tempo
dotnet run --project src/Web.Api
```

Run the full test suite (the integration tests spin up a throwaway PostgreSQL container, so
Docker must be running):

```bash
dotnet test SmartiePaws.slnx
```

To target .NET 8 or .NET 9 instead of .NET 10, see the notes in `Directory.Build.props`.

## Credit

Built from Milan Jovanović's [Clean Architecture template](https://www.milanjovanovic.tech),
including its `.claude/skills/` pack (`/add-feature`, `/add-entity`, `/add-tests`,
`/add-migration`, `/add-background-job`, `/bootstrap`, `/ca-review`).
