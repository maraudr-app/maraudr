using System.Net.Http.Json;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Options;

namespace Maraudr.User.Infrastructure.Repositories;

public class MailSenderRepository(HttpClient httpClient, IOptions<ApiSettings> options):IMailSenderRepository  

{
    public async Task SendWelcomeEmailTo(string email, string name)
    {
        
            var url = options.Value.EmailSenderApiUrl + "email/send-welcome";
            Console.WriteLine(url);
        
            var payload = new 
            {
                toEmail = email.Trim(),
                name = name
            };

            var response = await httpClient.PostAsJsonAsync(url, payload);
        
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Échec de l'envoi de l'email: {error}", null, response.StatusCode);
            }
    }
}    
