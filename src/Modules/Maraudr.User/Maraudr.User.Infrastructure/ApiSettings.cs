namespace Maraudr.User.Infrastructure;

public class ApiSettings
{
    public string EmailSenderApiUrl { get; set; } = null!;
    
    
    public string AssociationApiUrl { get; set; } = null!;
    
    public string EmailSenderApiKey { get; set; } = string.Empty;
    public string UserApiKey { get; set; } = string.Empty;
    

}