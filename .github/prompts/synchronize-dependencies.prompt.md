---
agent: agent
name: synchronize-dependencies
description: Check if packages are in Directory.Packages.props and synchronize with Aspire orchestration
---

## Synchronize Dependencies

1. Read and analyze Directory.Packages.props:
   - List all PackageVersion entries
   - Note the version for each package
   - Check for ManagePackageVersionsCentrally setting

2. Scan all project files (.csproj):
   - Identify all PackageReference entries
   - Check if they specify versions (they shouldn't with Central Package Management)
   - Look for packages not defined in Directory.Packages.props

3. Check Aspire orchestration requirements:
   - Verify Aspire.Hosting packages are present
   - Ensure OpenTelemetry packages are included
   - Check for ServiceDefaults dependencies
   - Validate resilience and service discovery packages

4. Synchronization actions:
   - Add missing packages to Directory.Packages.props
   - Remove version attributes from project PackageReference entries
   - Update outdated Aspire component versions
   - Ensure consistency across all projects

5. Report findings:
   - List packages missing from Directory.Packages.props
   - Identify version conflicts
   - Suggest Aspire packages to add
   - Show packages with hardcoded versions in projects

6. Required Aspire packages to check:
   - Aspire.Hosting (AppHost)
   - Aspire.Hosting.AppHost (AppHost)
   - Microsoft.Extensions.Http.Resilience (Services)
   - Microsoft.Extensions.ServiceDiscovery (Services)
   - OpenTelemetry.* packages (Observability)

Example workflow:
```
Step 1: Analyzing Directory.Packages.props...
✓ Found 11 package versions defined
✓ Central Package Management is enabled

Step 2: Scanning project files...
⚠ Found package with version in AntLogistics.Core.csproj:
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />

Step 3: Checking Aspire orchestration...
✓ Aspire.Hosting packages present
✓ OpenTelemetry packages configured
⚠ Missing: Aspire.Hosting.Redis (recommended for caching)

Step 4: Synchronization recommendations...

Add to Directory.Packages.props:
<PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
<PackageVersion Include="Aspire.Hosting.Redis" Version="13.0.0" />

Update AntLogistics.Core.csproj:
<PackageReference Include="Newtonsoft.Json" />  <!-- Remove Version attribute -->

Add to AntLogistics.AppHost.csproj:
<PackageReference Include="Aspire.Hosting.Redis" />
```

Example Directory.Packages.props structure:
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Aspire Hosting -->
    <PackageVersion Include="Aspire.Hosting" Version="13.0.0" />
    <PackageVersion Include="Aspire.Hosting.AppHost" Version="13.0.0" />
    
    <!-- Service Defaults -->
    <PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="9.0.0" />
    <PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="9.0.0" />
    
    <!-- OpenTelemetry -->
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.10.0" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.10.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.10.0" />
  </ItemGroup>
</Project>
```