---
name: aquaguard-workflow
description: Triage and coordinate AquaGuard changes across the .NET backend and React/Vite frontend, including specialist delegation, validation commands, and configuration safety.
---

# AquaGuard Cross-Stack Workflow

## Triage and delegation

- Read root `AGENTS.md` and `docs/context.md` before planning cross-stack work.
- Route backend work to `backend/src/Api`, `Application`, `Domain`, `Edge`, or `Infrastructure` according to ownership; the solution is `backend/AquaGuard.slnx`.
- Route browser UI and frontend assets to `frontend/aquaguard-web/` (`src/`, `public/`, `src/assets/`). Use the `frontend-specialist` agent for scoped React/Vite work when available.
- Split independent work into bounded tasks with non-overlapping write scopes. Align API or data contracts before parallel changes that touch both stacks; coordinator integrates and reviews the combined diff.

## Project conventions

- Keep domain entities and value objects in `AquaGuard.Domain`; keep HTTP concerns in Api and persistence/integrations in Infrastructure.
- Backend uses C#, four-space indentation, nullable reference types, and PascalCase public types and members.
- Frontend uses React/Vite with JavaScript/JSX, two-space indentation, PascalCase component names, camelCase variables/functions, and the existing ESLint configuration.

## Commands and validation

- Recommend applicable pre-submission checks and report their status accurately.
- Backend build: from `backend/`, run `dotnet build AquaGuard.slnx`.
- Frontend lint/build: from `frontend/aquaguard-web/`, run `npm run lint` and `npm run build`. `npm run dev` starts local development.
- There are currently no configured backend test projects or frontend test runner. Do not require nonexistent tests or imply they exist or ran.

## Configuration and security

- Never add secrets to tracked files. Use environment variables or local development configuration for credentials.
- Review `appsettings*.json` before configuration edits and exclude machine-specific values.
- Avoid committing generated output such as frontend `dist/` or .NET build directories. Keep changes scoped and inspect the final diff before reporting completion.
