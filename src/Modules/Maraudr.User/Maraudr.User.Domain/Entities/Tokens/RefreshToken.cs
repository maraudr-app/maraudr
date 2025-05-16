using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maraudr.Authentication.Domain.Entities;

namespace Maraudr.User.Domain.Entities.Tokens
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
