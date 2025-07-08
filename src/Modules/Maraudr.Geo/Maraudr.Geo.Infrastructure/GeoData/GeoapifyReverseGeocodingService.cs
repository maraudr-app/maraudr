using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Maraudr.Geo.Infrastructure.GeoData;

public interface IGeoAddressEnricher
{
    Task<string?> GetAddressAsync(double latitude, double longitude);
}

public class GeoapifyReverseGeocodingService(HttpClient httpClient, IConfiguration config) : IGeoAddressEnricher
{
    private readonly IConfiguration _config = config;

    public async Task<string?> GetAddressAsync(double latitude, double longitude)
    {
        
        var apiKey = Environment.GetEnvironmentVariable("GEOAPIFY_API_KEY");
        var url = $"https://api.geoapify.com/v1/geocode/reverse?lat={latitude}&lon={longitude}&apiKey={apiKey}";

        var response = await httpClient.GetFromJsonAsync<GeoapifyResponse>(url);
        return response?.Features?.FirstOrDefault()?.Properties?.Formatted;
    }

    private class GeoapifyResponse
    {
        public List<Feature>? Features { get; set; }
    }

    private class Feature
    {
        public Properties? Properties { get; set; }
    }

    private class Properties
    {
        public string? Formatted { get; set; }
    }
}
