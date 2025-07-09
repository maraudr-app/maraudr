using System.Net.Http.Json;
using Maraudr.Planning.Application;
using Maraudr.Planning.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.Planning.Infrastructure.Repositories;

using System;
using System.Collections.Generic;


public class EmailingRepository(HttpClient httpClient,IOptions<ApiSettings> options):IEmailingRepository
{
    public async Task SendEventEmailAsync(List<Guid> usersIds, string eventTitle, string eventDescription)
    {
        var url = options.Value.EmailSenderApiUrl + "email/send-event-notify-batch";
        Console.WriteLine("--------------------------------------------------");
        var baseUserUrl = options.Value.UserApiUrl + "api/users/";
        
        List<string> usersEmail = [];
        foreach (var id in usersIds)
        {
            var userUrl = baseUserUrl + id; 
            using var userRequest = new HttpRequestMessage(HttpMethod.Get, userUrl);
            userRequest.Headers.Add("X-API-KEY", options.Value.UserApiKey);
            var responseFromUser = await httpClient.SendAsync(userRequest);
            responseFromUser.EnsureSuccessStatusCode();
            var user = await responseFromUser.Content.ReadFromJsonAsync<UserDto>();
            usersEmail.Add(user.ContactInfo.Email);
        }
        var payload = new 
        {
            userData = usersEmail,
            eventTitle = eventTitle,
            eventDescription= eventDescription
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