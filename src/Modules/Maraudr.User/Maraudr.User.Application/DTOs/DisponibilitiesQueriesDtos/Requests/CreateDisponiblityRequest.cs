using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DisponibilitiesQueriesDtos.Requests;

public class CreateDisponiblityRequest
{    
    [Required]
    public DateTime Start { get; set; }
    
    [Required]
    public DateTime End { get; set; }
    
    [Required]
    public Guid AssociationId { get; set; }
}