using System.Security.Claims;

namespace Maraudr.User.Domain.ValueObjects.Tokens;


public class ValidateTokenResponse
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; }
    public ClaimsPrincipal Principal { get; private set; }

    private ValidateTokenResponse(bool isValid, ClaimsPrincipal principal = null, string errorMessage = null)
    {
        IsValid = isValid;
        Principal = principal;
        ErrorMessage = errorMessage;
    }

    public static ValidateTokenResponse Successful(ClaimsPrincipal principal)
    {
        return new ValidateTokenResponse(true, principal);
    }

    public static ValidateTokenResponse Failed(string errorMessage)
    {
        return new ValidateTokenResponse(false, errorMessage: errorMessage);
    }
}