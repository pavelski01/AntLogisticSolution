namespace AntLogistics.Core.Data.Models;

/// <summary>
/// Defines the available roles for system operators.
/// </summary>
public enum OperatorRole
{
    /// <summary>
    /// Standard operator with limited permissions (read/write operations on assigned resources).
    /// </summary>
    Operator = 0,

    /// <summary>
    /// Administrator with full system permissions.
    /// </summary>
    Admin = 1
}
