namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Represents a system operator/user in the logistics system.
/// </summary>
public class Operator
{
    /// <summary>
    /// Gets or sets the unique identifier for the operator.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username for login.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password hash (Bcrypt).
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the operator.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role of the operator (Operator or Admin).
    /// </summary>
    public OperatorRole Role { get; set; } = OperatorRole.Operator;

    /// <summary>
    /// Gets or sets the idle timeout in minutes (5-180).
    /// </summary>
    public int IdleTimeoutMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether the operator is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the timestamp of the last successful login.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of sessions for this operator.
    /// </summary>
    public ICollection<OperatorSession> Sessions { get; set; } = new List<OperatorSession>();

    /// <summary>
    /// Gets or sets the collection of readings created by this operator.
    /// </summary>
    public ICollection<Reading> Readings { get; set; } = new List<Reading>();
}
