namespace Maraudr.User.Domain.ValueObjects.Tokens;

public class RefreshTokenResponse
{
    public bool Success { get; private set; }
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public List<string> Errors { get; private set; } = [];

    public static RefreshTokenResponse Successful(string accessToken, DateTime expiresAt) =>
        new() { Success = true, AccessToken = accessToken, ExpiresAt = expiresAt };

    public static RefreshTokenResponse Failed(List<string> errors) =>
        new() { Success = false, Errors = errors };
}