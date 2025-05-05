using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests

{
    public class UserIdRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}