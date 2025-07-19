
namespace Maraudr.User.Domain.Entities.Tokens
{
    public class PasswordResetToken
    {
        public PasswordResetToken() { }
    
        public PasswordResetToken(Guid userId, string token, TimeSpan expiration)
        {
            UserId = userId;
            Token = token;
            CreatedAt = DateTime.UtcNow; 
            ExpiresAt = DateTime.UtcNow.Add(expiration);
            IsUsed = false;
        }
    
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        
        public bool IsUsed { get; set; }
    }
}
