# Custom GitHub Copilot Commands

This file contains custom prompt commands for the AntLogisticSolution project.

## Rewrite Code

```prompt
name: rewrite
description: Rewrite the selected code to improve readability and keep functionality
tags: refactor, readability, clean-code

When the user requests to rewrite code:

1. Analyze the selected code for:
   - Readability issues
   - Naming clarity
   - Code structure
   - Best practices violations

2. Rewrite the code to:
   - Improve variable and method names
   - Simplify complex expressions
   - Extract magic numbers to constants
   - Break down long methods
   - Remove code duplication
   - Apply SOLID principles
   - Use modern C# features (pattern matching, null-coalescing, etc.)

3. Maintain:
   - Original functionality
   - Performance characteristics
   - Public API contracts
   - Existing tests compatibility

4. Follow project standards:
   - Use file-scoped namespaces
   - Enable nullable reference types
   - Add XML documentation if missing
   - Use async/await properly
   - Apply proper error handling

Example transformation:
```csharp
// Before
public string GetData(int id)
{
    var x = _repo.Get(id);
    if (x == null)
        return null;
    return x.Name + " - " + x.Code;
}

// After
/// <summary>
/// Retrieves formatted data for the specified identifier.
/// </summary>
public string? GetFormattedData(int id)
{
    var entity = _repository.GetById(id);
    return entity is not null 
        ? $"{entity.Name} - {entity.Code}" 
        : null;
}
```
```

## Add Comments

```prompt
name: add-comments
description: Add helpful comments to the selected code
tags: documentation, comments, readability

When the user requests to add comments:

1. Determine the comment style (default to concise if not specified):
   - <style>Concise</style>: Brief, to-the-point comments for experienced developers
   - <style>Verbose</style>: Detailed explanations with context and reasoning
   - <style>Beginner-friendly</style>: Step-by-step explanations with educational details

2. Add comments for:
   - Complex algorithms or logic
   - Non-obvious business rules
   - Why certain decisions were made (not what the code does)
   - Performance considerations
   - Edge cases being handled
   - Public APIs (XML documentation)

3. Follow commenting guidelines:
   - Use XML documentation (///) for public members
   - Use single-line comments (//) for inline explanations
   - Avoid stating the obvious
   - Keep comments up to date with code
   - Explain WHY, not WHAT
   - Add TODO/HACK/NOTE markers when appropriate

4. Style variations:

<style>Concise style:</style>
```csharp
// Validate input before processing
if (order is null) throw new ArgumentNullException(nameof(order));

// Calculate total with discount applied
var total = order.Items.Sum(i => i.Price * i.Quantity) * (1 - discount);
```

<style>Verbose style:</style>
```csharp
// Validate the order parameter to ensure it's not null. This prevents
// NullReferenceExceptions later in the method and provides a clear error
// message at the point of failure, making debugging easier.
if (order is null) throw new ArgumentNullException(nameof(order));

// Calculate the order total by summing all item subtotals (price × quantity),
// then applying the discount percentage. The discount is subtracted from 1
// to get the multiplier (e.g., 10% discount = 0.9 multiplier).
var total = order.Items.Sum(i => i.Price * i.Quantity) * (1 - discount);
```

<style>Beginner-friendly style:</style>
```csharp
// Step 1: Check if the order object exists
// We need to validate input to prevent errors. If order is null,
// we throw an exception with a descriptive message.
if (order is null) throw new ArgumentNullException(nameof(order));

// Step 2: Calculate the total cost
// First, we multiply each item's price by its quantity to get the subtotal.
// Then, we sum all subtotals using LINQ's Sum method.
// Finally, we apply the discount by multiplying by (1 - discount).
// Example: If discount is 0.1 (10%), we multiply by 0.9 (90%).
var total = order.Items.Sum(i => i.Price * i.Quantity) * (1 - discount);
```

Example with XML documentation:
```csharp
/// <summary>
/// Processes an order and calculates the final total with discounts applied.
/// </summary>
/// <param name="order">The order to process. Must not be null.</param>
/// <param name="discount">The discount rate as a decimal (0.1 = 10%).</param>
/// <returns>The final order total after discount.</returns>
/// <exception cref="ArgumentNullException">Thrown when order is null.</exception>
public decimal ProcessOrder(Order order, decimal discount)
{
    // Validation and calculation...
}
```

## Synchronize Dependencies

```prompt
name: synchronize-dependencies
description: Check if packages are in Directory.Packages.props and synchronize with Aspire orchestration
tags: dependencies, packages, aspire, nuget

When the user requests to synchronize dependencies:

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
```

---

## Usage

To use these custom commands with GitHub Copilot:

### Rewrite Command
Select code and ask:
- "Rewrite this code for better readability"
- "Apply the rewrite command to improve this method"
- "Refactor this using the rewrite guidelines"

### Add Comments Command
Select code and ask:
- "Add comments style:concise"
- "Add comments style:verbose"
- "Add comments style:beginner-friendly"
- "Add helpful comments to explain this logic"

### Synchronize Dependencies Command
Ask in the chat:
- "Synchronize dependencies across the solution"
- "Check if packages are in Directory.Packages.props"
- "Validate Aspire orchestration dependencies"
- "Find missing packages in central package management"

These commands ensure consistency across the codebase and adherence to project standards.
