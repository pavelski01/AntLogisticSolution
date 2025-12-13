using AntLogistics.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace AntLogistics.Core.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly AntLogisticsDbContext _db;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(AntLogisticsDbContext db, ILogger<AuthorizationService> logger)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var normalized = username.Trim().ToLowerInvariant();

        var op = await _db.Operators.SingleOrDefaultAsync(o => o.Username == normalized && o.IsActive, cancellationToken);
        if (op is null)
        {
            _logger.LogWarning("Failed login for {Username}", normalized);
            return false;
        }

        var ok = BCrypt.Net.BCrypt.Verify(password, op.PasswordHash);
        if (ok)
        {
            op.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Operator {Username} logged in", normalized);
        }
        else
        {
            _logger.LogWarning("Failed login for {Username}", normalized);
        }

        return ok;
    }
}
