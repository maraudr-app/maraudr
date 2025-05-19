using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Domain.Entities
{
    public class PasswordResetToken(Guid userId, string token, TimeSpan expiration) : AbstractToken(userId, token, DateTime.UtcNow.Add(expiration))
    {

        public bool IsUsed { get; private set; } = false;

        public bool IsValid => !IsUsed && ExpiresAt > DateTime.UtcNow;

        public void MarkAsUsed()
        {
            IsUsed = true;
        }
    }
}
