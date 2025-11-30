# AntLogistics.UI

React-based Single Page Application (SPA) for the Ant Logistics solution.

## Technology Stack

- **Frontend Framework**: React 18.3
- **Build Tool**: Vite 6.0
- **Routing**: React Router DOM 7.1
- **Language**: JavaScript (ES2020+)
- **Backend Integration**: ASP.NET Core SPA Services

## Project Structure

```
AntLogistics.UI/
├── ClientApp/                  # React application
│   ├── src/
│   │   ├── pages/              # Page components
│   │   │   ├── Home.jsx
│   │   │   ├── Warehouses.jsx
│   │   │   └── pages.css
│   │   ├── App.jsx             # Main App component
│   │   ├── App.css             # App styles
│   │   ├── main.jsx            # React entry point
│   │   └── index.css           # Global styles
│   ├── index.html              # HTML template
│   ├── vite.config.js          # Vite configuration
│   ├── package.json            # npm dependencies
│   └── eslint.config.js        # ESLint configuration
├── Program.cs                  # ASP.NET Core backend
├── AntLogistics.UI.csproj      # Project file
└── appsettings.json            # Configuration

```

## Development Setup

### Prerequisites

- Node.js (v18 or later)
- npm (comes with Node.js)
- .NET 10.0 SDK

### Running the Application

### ⚠️ Important: Stop Running Processes First

If you get file locking errors when building, stop all running .NET processes:

```powershell
# Stop the Aspire AppHost and all services
# Press Ctrl+C in the terminal where AppHost is running
# Or stop the debugging session in VS Code
```

#### Using .NET Aspire (Recommended)

Run the entire solution through the AppHost:

```powershell
cd c:\01_REPO\AntLogisticSolution
dotnet run --project src/AntLogistics.AppHost
```

The UI will be available at the URL shown in the Aspire dashboard.

#### Standalone Development

1. **Start the React Development Server:**

```powershell
cd src/AntLogistics.UI/ClientApp
npm run dev
```

The React app will run on `http://localhost:5173`

2. **Start the ASP.NET Core Backend:**

```powershell
cd src/AntLogistics.UI
dotnet run
```

The backend will proxy requests to the React dev server.

### Building for Production

```powershell
cd src/AntLogistics.UI/ClientApp
npm run build
```

This creates optimized production files in `ClientApp/dist/`.

## Features

### Current Features

- 🏠 **Home Page**: Landing page with feature overview
- 📦 **Warehouses**: Warehouse management interface (with mock data)
- 🎨 **Responsive Design**: Modern gradient-based UI
- 🔄 **Client-Side Routing**: React Router for SPA navigation

### Planned Features

- Integration with AntLogistics.Core API
- Real-time warehouse data
- Commodity tracking interface
- Analytics dashboard
- User authentication

## API Integration

The application is configured to proxy API requests to the backend:

```javascript
// vite.config.js
proxy: {
  '/api': {
    target: 'http://localhost:5002',
    changeOrigin: true,
    secure: false,
  }
}
```

Example API call:

```javascript
const response = await fetch('/api/warehouses')
const data = await response.json()
```

## Configuration

### Development

- **React Dev Server Port**: 5173 (Vite)
- **ASP.NET Core Port**: 5002 (HTTP), 7002 (HTTPS)
- **API Proxy**: Configured in `vite.config.js`

### Production

The production build is served by ASP.NET Core from `ClientApp/dist/`.

## Aspire Integration

The UI project is registered in the Aspire orchestration:

```csharp
var ui = builder.AddProject<Projects.AntLogistics_UI>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(coreApi);
```

This provides:
- Service discovery
- Health checks
- Distributed tracing
- Centralized logging
- Resilient HTTP communication

## Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Dependencies

### Production Dependencies

- `react` ^18.3.1
- `react-dom` ^18.3.1
- `react-router-dom` ^7.1.1

### Development Dependencies

- `vite` ^6.0.5
- `@vitejs/plugin-react` ^4.3.4
- `eslint` ^9.17.0
- `typescript` ^5.7.2

## Styling

The application uses vanilla CSS with:
- Modern gradient backgrounds
- Responsive grid layouts
- Dark mode by default
- Clean, professional design

## Best Practices

- ✅ Use functional components with hooks
- ✅ Follow React 18+ patterns
- ✅ Implement proper error handling
- ✅ Use async/await for API calls
- ✅ Organize code by feature (pages)
- ✅ Follow ESLint recommendations

## Troubleshooting

### Port Already in Use

If port 5173 is in use:

```powershell
# Change the port in vite.config.js
server: {
  port: 5174,  // or any available port
}
```

### npm install fails

```powershell
# Clear npm cache
npm cache clean --force
npm install
```

### Build errors

```powershell
# Clean and rebuild
Remove-Item -Recurse -Force node_modules, dist
npm install
npm run build
```

## Contributing

Follow the guidelines in `.github/copilot-instructions.md` for coding standards and best practices.

---

Built with ❤️ using .NET Aspire and React
