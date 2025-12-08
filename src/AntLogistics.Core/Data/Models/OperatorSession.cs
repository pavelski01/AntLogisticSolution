namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents an active session for an operator.
/// </summary>
public class OperatorSession
{
    /// <summary>
    /// Gets or sets the unique identifier for the session.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the operator identifier (foreign key).
    /// </summary>
    public Guid OperatorId { get; set; }

    /// <summary>
    /// Gets or sets the unique session token.
    /// </summary>
    public Guid SessionToken { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the session was issued.
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last activity (sliding expiration checkpoint).
    /// </summary>
    public DateTime LastSeenAt { get; set; }

    /// <summary>
    /// Gets or sets the absolute expiration timestamp for the session.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the session was revoked (if applicable).
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the client IP address for audit purposes.
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// Gets or sets the user agent string.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the associated operator (navigation property).
    /// </summary>
    public virtual Operator Operator { get; set; } = null!;
}
