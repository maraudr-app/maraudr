using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Domain.Entities
{
    public class PasswordResetToken
    {
        public PasswordResetToken() { }
    
        public PasswordResetToken(Guid userId, string token, TimeSpan expiration)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = DateTime.UtcNow.Add(expiration); 
        }
    
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
