namespace Application.DTOs;

public class InvitatonTokenDto
{
    public String InvitedByFirstName { get; set; }
    public string invitedByLastname { get; set; }
    public Guid AssociationId { get; set; }
}