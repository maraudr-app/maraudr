namespace Maraudr.User.Domain.Entities.Tokens;

public class InvitationToken
{
    public Guid Id { get; set; }
    public Guid InvitedByUserId { get; set; } 
    public string InvitedEmail { get; set; }   
    public Guid AssociationId { get; set; }

    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public Guid? CreatedUserId { get; set; } 
        
}