---
agent: agent
name: add-comments
description: Add helpful comments to the selected code
---

## Add Comments

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