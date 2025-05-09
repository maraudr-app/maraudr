using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Application.DTOs.Responses
{
    public class AuthResponseDTO
    {
       
            public bool Success { get; private set; }
            public string? AccessToken { get; private set; }
            public string? RefreshToken { get; private set; }
            public int ExpiresIn { get; private set; }
            public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();

            private AuthResponseDTO() { }

            public static AuthResponseDTO Successful(string accessToken, string refreshToken, int expiresIn)
                => new AuthResponseDTO
                {
                    Success = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = expiresIn
                };

            public static AuthResponseDTO Failed(params string[] errors)
                => new AuthResponseDTO
                {
                    Success = false,
                    Errors = errors
                };
        }
    }
}
