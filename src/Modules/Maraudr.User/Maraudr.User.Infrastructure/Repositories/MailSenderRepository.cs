using System.Net.Http.Json;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Options;

namespace Maraudr.User.Infrastructure.Repositories;

public class MailSenderRepository(HttpClient httpClient, IOptions<ApiSettings> options):IMailSenderRepository  

{
    public async Task SendWelcomeEmailTo(string email, string name)
    {
        
            var url = options.Value.EmailSenderApiUrl + "email/send-welcome";
        
            var payload = new 
            {
                toEmail = email.Trim(),
                name = name
            };

          
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = JsonContent.Create(payload);

            request.Headers.Add("X-API-KEY", options.Value.EmailSenderApiKey);
        
            var response = await httpClient.SendAsync(request);
        
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Échec de l'envoi de l'email: {error}", null, response.StatusCode);
            }
    }

    public async Task SendResetEmailAsync(string name,string email,  string token)
    {
        var url = options.Value.EmailSenderApiUrl + "email/send-reset-link";
        
        var payload = new 
        {
            toEmail = email.Trim(),
            name = name,
            token= token
        };
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = JsonContent.Create(payload);

        request.Headers.Add("X-API-KEY", options.Value.EmailSenderApiKey);
        
        var response = await httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Échec de l'envoi de l'email: {error}", null, response.StatusCode);
        }
    }
}    
