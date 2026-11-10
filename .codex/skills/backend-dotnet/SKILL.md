---
name: backend-dotnet
description: Implement or review AquaGuard's .NET backend across API, application, domain, infrastructure, and edge projects. Use for C#, ASP.NET Core, sensor data, ingestion, persistence, and backend configuration.
---

# AquaGuard .NET Backend

## Before changing code

- Read the repository `AGENTS.md` and `docs/context.md` for repository conventions and domain goals.
- Inspect the relevant project and its existing patterns before adding abstractions. The solution is `backend/AquaGuard.slnx`; projects are under `backend/src/`.
- Treat assumptions about physical sensors, calibration, units, and thresholds as explicit decisions. Do not invent real-world values.

## Layering

- `AquaGuard.Domain`: entities, value objects, domain rules, and domain exceptions. Keep it independent of ASP.NET Core, databases, and device transports.
- `AquaGuard.Application`: use cases and orchestration. Express operations in domain terms and depend on abstractions where external capabilities are needed.
- `AquaGuard.Api`: ASP.NET Core HTTP surface, request/response mapping, and application composition.
- `AquaGuard.Infrastructure`: persistence and external integrations. Keep provider-specific details here.
- `AquaGuard.Edge`: device-facing or ingestion concerns; preserve transport and measurement metadata rather than silently normalizing away meaning.

Keep received, validated, processed, and user-facing sensor data distinguishable. Make units explicit, validate physical ranges, and favor explainable deterministic rules for anomaly and alert behavior.

## C# style

- Target the existing `net10.0` projects; nullable reference types and implicit usings are enabled.
- Use four-space indentation, PascalCase for types and public members, and descriptive domain names.
- Follow neighboring code and keep each responsibility in its owning project. Avoid framework dependencies in Domain and unnecessary abstraction or technology additions.

## Build and validation

- Before submitting backend changes, build the solution from `backend/` with `dotnet build AquaGuard.slnx` where feasible.
- Run the API locally from `backend/`: `dotnet run --project src/AquaGuard.Api`.
- No backend test project is currently configured. Do not claim a test suite exists or that tests were run. If tests are introduced, document their exact command and report only tests actually run.

## Configuration and security

- Never commit credentials, tokens, or environment-specific secrets. Use environment variables or local development configuration.
- Review `appsettings*.json` before changing configuration; keep safe defaults and avoid machine-specific values.
- Validate incoming payloads and consider duplicates, delayed readings, disconnected sensors, timeouts, retries, and idempotency when relevant. Bound retries and use backoff where applicable.
