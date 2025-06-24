using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Maraudr.EmailSender.Endpoints.Identity;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["ApiKey"];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Clé API manquante");
            return;
        }

        if (!_apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Clé API non valide");
            return;
        }

        await _next(context);
    }
}

    public static class ApiKeyAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthMiddleware>();
        }
    }
