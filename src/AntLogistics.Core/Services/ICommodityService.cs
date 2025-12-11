using AntLogistics.Core.Dto;

namespace AntLogistics.Core.Services;

/// <summary>
/// Service interface for commodity operations.
/// </summary>
public interface ICommodityService
{
    /// <summary>
    /// Creates a new commodity.
    /// </summary>
    /// <param name="request">The commodity creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created commodity response.</returns>
    Task<CommodityResponse> CreateCommodityAsync(CreateCommodityRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a commodity by its unique identifier.
    /// </summary>
    /// <param name="id">The commodity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The commodity if found; otherwise, null.</returns>
    Task<CommodityResponse?> GetCommodityByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all commodities.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive commodities.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of commodity responses.</returns>
    Task<IEnumerable<CommodityResponse>> GetAllCommoditiesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a commodity by its SKU.
    /// </summary>
    /// <param name="sku">The commodity SKU.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The commodity if found; otherwise, null.</returns>
    Task<CommodityResponse?> GetCommodityBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
