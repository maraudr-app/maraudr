using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DisponibilitiesQueriesDtos.Requests;

public class UpdateDisponiblityRequest
{
        [Required]
        public DateTime Start { get; set; }
    
        [Required]
        public DateTime End { get; set; }

}