using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UsersQueriesDtos.Requests

{
    public class UserIdRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}