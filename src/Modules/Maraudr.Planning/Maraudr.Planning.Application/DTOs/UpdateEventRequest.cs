namespace Maraudr.Planning.Application.DTOs;

public class UpdateEventRequest
{
    
    
        public List<Guid>? ParticipantsIds { get; set; }
        public DateTime? BeginningDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }

    
}