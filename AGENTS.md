# Repository Guidelines

## Project Structure & Modules

The repository has two main applications. The .NET solution is in `backend/AquaGuard.slnx`; its projects are under `backend/src/` (`Api`, `Application`, `Domain`, `Edge`, and `Infrastructure`). Domain entities and value objects belong in `AquaGuard.Domain`. The React/Vite frontend is in `frontend/aquaguard-web/`: application code is in `src/`, static files in `public/`, and frontend assets in `src/assets/`. Shared project context is documented in `docs/context.md`.

## Build, Test, and Development

Run commands from the relevant application directory:

- `cd frontend/aquaguard-web && npm install` installs frontend dependencies.
- `npm run dev` starts the Vite development server; `npm run build` creates a production bundle in `dist/`.
- `npm run lint` runs ESLint across the frontend; `npm run preview` serves the built bundle locally.
- `cd backend && dotnet build AquaGuard.slnx` builds all .NET projects; `dotnet run --project src/AquaGuard.Api` starts the API.

No test projects or frontend test runner are currently configured. Add tests with the project-specific framework when introducing test coverage, and document the run command alongside the relevant project.

## Coding Style & Naming

Follow existing conventions: C# uses four-space indentation, nullable reference types, PascalCase for types and public members, and descriptive domain names. React files use JavaScript/JSX and follow the ESLint rules in `eslint.config.js`; use two-space indentation, PascalCase for components, and camelCase for variables and functions. Keep code in the project that owns its responsibility and avoid committing generated output such as `dist/` or .NET build directories.

## Testing Guidelines

Run `npm run lint` and the applicable build command before submitting changes. For backend changes, build `AquaGuard.slnx`; for frontend changes, run `npm run build`. When tests are added, use recognizable names such as `PropertyTests.cs` or `Property.test.jsx` and include the exact test command in the change description.

## Commits & Pull Requests

Recent history uses short, imperative-style subjects, often with prefixes such as `feat:` and `chore:`. Keep commits focused and explain the user-visible or architectural purpose. Pull requests should summarize the change, note validation commands and results, link related issues when available, and include screenshots for visible frontend changes.

## Configuration

Keep secrets out of tracked files. Use local development configuration or environment variables for credentials, and review `appsettings*.json` before adding sensitive or machine-specific values.
