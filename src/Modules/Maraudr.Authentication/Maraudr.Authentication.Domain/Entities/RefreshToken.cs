using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Domain.Entities
{
    public class RefreshToken:AbstractToken
    {

        public bool IsRevoked { get; private set; }
        public string? RevokedReason { get; private set; }


        public RefreshToken(Guid userId, string token, DateTime expiresAt) :
            base(userId, token, expiresAt) => IsRevoked = false;
        // Add EF spec

        public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

        public void Revoke(string reason = "Token manually revoked")
        {
            IsRevoked = true;
            RevokedReason = reason;
        }
    }
}
