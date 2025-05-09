using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Application.DTOs.Responses
{
    public class RegisterResponse
    {

       
            public bool Success { get; private set; }
            public Guid? UserId { get; private set; }
            public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();

            private RegisterResponse() { }

            public static RegisterResponse Successful(Guid userId)
                => new RegisterResponse { Success = true, UserId = userId };

            public static RegisterResponse Failed(params string[] errors)
                => new RegisterResponse { Success = false, Errors = errors };
        
    }
}
