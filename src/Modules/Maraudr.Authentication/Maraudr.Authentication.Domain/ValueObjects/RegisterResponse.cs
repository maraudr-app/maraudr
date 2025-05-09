using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Application.DTOs.Responses
{
    public class RegisterResponseDTO
    {

       
            public bool Success { get; private set; }
            public Guid? UserId { get; private set; }
            public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();

            private RegisterResponseDTO() { }

            public static RegisterResponseDTO Successful(Guid userId)
                => new RegisterResponseDTO { Success = true, UserId = userId };

            public static RegisterResponseDTO Failed(params string[] errors)
                => new RegisterResponseDTO { Success = false, Errors = errors };
        
    }
}
