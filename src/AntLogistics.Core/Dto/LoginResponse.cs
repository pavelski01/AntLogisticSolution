namespace AntLogistics.Core.Dto;

public record LoginResponse
{
    public required bool Success { get; init; }
    public string? Username { get; init; }
    public string? Token { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
