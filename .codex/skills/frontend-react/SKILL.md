---
name: frontend-react
description: Build and maintain the AquaGuard React/Vite frontend using its existing JSX, CSS, responsive layout, and ESLint conventions.
---

# AquaGuard React/Vite Frontend

## Before editing

- Read the root `AGENTS.md` and inspect relevant files under `frontend/aquaguard-web/src/`.
- Check `package.json` and `eslint.config.js` before changing scripts or lint-sensitive code.
- Keep frontend changes in `frontend/aquaguard-web/` unless the task requires a related root configuration change.

## Structure and components

- `src/main.jsx` mounts the app and imports global styles; `src/App.jsx` is the main screen.
- Put reusable React components in `src/` and colocate component-specific styles where existing conventions support it.
- Use JavaScript/JSX, functional components, and React hooks. Name components in PascalCase and variables/functions in camelCase. Keep components focused and state local unless shared state is needed.
- Use semantic HTML, keyboard-accessible controls, descriptive accessible names, and meaningful `alt` text. Use empty `alt` for decorative images.
- Store imported frontend assets in `src/assets/`; static public files belong in `public/` and are referenced by root-relative URLs.

## Styling

- Use the existing plain CSS files (`src/index.css`, `src/App.css`) and CSS custom properties. Preserve responsive behavior and the current light/dark color scheme.
- Follow the existing two-space indentation and CSS nesting patterns supported by the project toolchain. Avoid adding a styling dependency for isolated changes.

## ESLint and build

- Follow `eslint.config.js`, including the recommended JavaScript, React Hooks, and React Refresh rules. Do not suppress lint rules without a specific reason.
- Before submission, from `frontend/aquaguard-web/`, run `npm run lint` and `npm run build` when feasible. Report commands not run and why.
- `npm run dev` starts the Vite development server.
- No frontend test runner is currently configured; no nonexistent test command is required.
