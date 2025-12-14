# Ant Logistics UI - Astro Frontend

This is the frontend application for Ant Logistics, built with Astro 5 and React.

## Tech Stack

- **Astro** v5.13+ - Modern web framework for building fast, content-focused websites
- **React** v18.3 - UI library for building interactive components
- **TypeScript** v5 - Type-safe JavaScript
- **Tailwind CSS** v4.1+ - Utility-first CSS framework
- **.NET Aspire** - Backend integration

## Prerequisites

- Node.js v18+
- npm (comes with Node.js)

## Getting Started

1. Install dependencies:

```bash
npm install
```

2. Run the development server:

```bash
npm run dev
```

The app will be available at `http://localhost:5173`

3. Build for production:

```bash
npm run build
```

4. Preview production build:

```bash
npm run preview
```

## Available Scripts

- `npm run dev` - Start development server on port 5173
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
- `npm run lint:fix` - Fix ESLint issues
- `npm run format` - Format code with Prettier

## Project Structure

```
ClientApp/
├── src/
│   ├── layouts/          # Astro layouts
│   │   └── BaseLayout.astro
│   ├── pages/            # Astro pages (file-based routing)
│   │   ├── index.astro   # Home page
│   │   └── warehouses.astro
│   ├── components/       # React components
│   │   └── WarehouseList.tsx
│   ├── styles/           # Global styles
│   │   └── globals.css
│   └── env.d.ts          # TypeScript definitions
├── public/               # Static assets
├── astro.config.mjs      # Astro configuration
├── tsconfig.json         # TypeScript configuration
├── package.json          # Dependencies and scripts
└── README.md             # This file
```

## Key Features

- **Server-Side Rendering (SSR)**: Configured with `@astrojs/node` adapter
- **React Integration**: Interactive components with `client:load` directive
- **Tailwind CSS v4**: Modern utility-first styling with `@tailwindcss/vite` plugin
- **API Proxy**: Configured to proxy `/api` requests to `http://localhost:5002`
- **TypeScript**: Full type safety with strict mode
- **ESLint & Prettier**: Code quality and formatting

## API Integration

The app is configured to proxy API requests to the backend:

```typescript
// astro.config.mjs
vite: {
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5002",
        changeOrigin: true,
        secure: false,
      },
    },
  },
}
```

To use the API in your components:

```typescript
const response = await fetch("/api/warehouses");
const data = await response.json();
```

## Astro Pages vs React Components

- **Astro Pages** (`src/pages/*.astro`): Static content, server-rendered
- **React Components** (`src/components/*.tsx`): Interactive UI elements

Use the `client:load` directive to hydrate React components:

```astro
<WarehouseList client:load />
```

## Development Notes

- Astro uses file-based routing: `src/pages/about.astro` → `/about`
- Astro components are zero-JS by default (only React components with `client:*` directives send JS)
- Tailwind v4 uses the new `@import "tailwindcss"` syntax
- The app runs on port 5173 to maintain compatibility with the existing .NET backend proxy configuration

## Learn More

- [Astro Documentation](https://docs.astro.build)
- [React Documentation](https://react.dev)
- [Tailwind CSS v4](https://tailwindcss.com/blog/tailwindcss-v4-beta)
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire)
