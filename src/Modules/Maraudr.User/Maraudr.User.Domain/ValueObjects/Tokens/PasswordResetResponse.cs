using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Domain.ValueObjects
{
    public class PasswordResetResponse
    {
        public bool Success { get; private set; }
        public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();

        private PasswordResetResponse() { }

        public static PasswordResetResponse Successful()
            => new PasswordResetResponse { Success = true };

        public static PasswordResetResponse Failed(params string[] errors)
            => new PasswordResetResponse { Success = false, Errors = errors };
    }
    
   
}
