using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Domain.Entities
{

    public abstract class AbstractToken(Guid userId, string token, DateTime expiresAt)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; } = userId;
        public string Token { get; private set; } = token;
        public DateTime ExpiresAt { get; private set; } = expiresAt;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        
    }

    
}
