
namespace Maraudr.Authentication.Domain.ValueObjects
{
    public class AuthResponse
    {
       
            public bool Success { get; private set; }
            public string? AccessToken { get; private set; }
            public string? RefreshToken { get; private set; }
            public int ExpiresIn { get; private set; }
            public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();

            private AuthResponse() { }

            public static AuthResponse Successful(string accessToken, string refreshToken, int expiresIn)
                => new AuthResponse
                {
                    Success = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = expiresIn
                };

            public static AuthResponse Failed(params string[] errors)
                => new AuthResponse
                {
                    Success = false,
                    Errors = errors
                };
        }
    
}
