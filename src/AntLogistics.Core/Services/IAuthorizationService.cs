namespace AntLogistics.Core.Services;

public interface IAuthorizationService
{
    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
}
