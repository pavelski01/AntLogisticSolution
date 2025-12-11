using AntLogistics.Core.Dto;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service interface for warehouse operations.
/// </summary>
public interface IWarehouseService
{
    /// <summary>
    /// Creates a new warehouse.
    /// </summary>
    /// <param name="request">The warehouse creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created warehouse response.</returns>
    Task<WarehouseResponse> CreateWarehouseAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a warehouse by its unique identifier.
    /// </summary>
    /// <param name="id">The warehouse identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The warehouse if found; otherwise, null.</returns>
    Task<WarehouseResponse?> GetWarehouseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all warehouses.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive warehouses.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of warehouse responses.</returns>
    Task<IEnumerable<WarehouseResponse>> GetAllWarehousesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a warehouse by its code.
    /// </summary>
    /// <param name="code">The warehouse code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The warehouse if found; otherwise, null.</returns>
    Task<WarehouseResponse?> GetWarehouseByCodeAsync(string code, CancellationToken cancellationToken = default);
}
