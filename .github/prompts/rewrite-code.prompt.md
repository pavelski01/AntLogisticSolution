---
agent: agent
name: rewrite-code
description: Rewrite the selected code for improved readability, maintainability, and adherence to best practices
---

## Rewrite Code

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
public string? GetFormattedData(int id)
{
    var entity = _repository.GetById(id);
    return entity is not null 
        ? $"{entity.Name} - {entity.Code}" 
        : default;
}
```