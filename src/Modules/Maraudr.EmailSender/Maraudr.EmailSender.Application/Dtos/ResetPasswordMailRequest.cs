namespace Maraudr.EmailSender.Application.Dtos;

public class ResetPasswordMailRequest
{
    public string ToEmail { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    
}