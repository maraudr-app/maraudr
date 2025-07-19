using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.InvitationDto;

public class SendInvitationRequest
{
    
    
    [Required]
    public string InvitedEmail { get; set; }
    
    [Required]
    public Guid AssociationId{ get; set; }
    
    [Required]
    public string? Message { get; set; } 
}