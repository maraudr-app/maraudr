namespace Maraudr.User.Domain.ValueObjects.Tokens;

public class UserInfoFromTokenResponse
{
    public bool Success { get; private set; }
    public Guid? UserId { get; private set; }
    public string? Email { get; private set; }
    public List<string> Errors { get; private set; } = [];

    public static UserInfoFromTokenResponse Successful(Guid userId, string? email) =>
        new() { Success = true, UserId = userId, Email = email };

    public static UserInfoFromTokenResponse Failed(List<string> errors) =>
        new() { Success = false, Errors = errors };
}