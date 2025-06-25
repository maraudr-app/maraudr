using System.Globalization;
using System.Net.Http.Json;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using Maraudr.Geo.Infrastructure.GeoData;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Point = NetTopologySuite.Geometries.Point;

namespace Maraudr.Geo.Infrastructure;

public class GeoRepository : IGeoRepository
{
    private readonly GeoContext _context;

    public GeoRepository(GeoContext context)
    {
        _context = context;
    }

    public async Task AddEventAsync(Domain.Entities.GeoData geoEvent)
    {
        _context.Add(geoEvent);
        _context.Entry(geoEvent).Property("Location").CurrentValue = new Point(geoEvent.Longitude, geoEvent.Latitude) { SRID = 4326 };
        await _context.SaveChangesAsync();
    }

    public async Task<List<Domain.Entities.GeoData>> GetEventsAsync(Guid associationId, DateTime from)
    {
        return await _context.GeoEvents
            .Where(e => e.GeoStore!.AssociationId == associationId && e.ObservedAt >= from)
            .ToListAsync();
    }

public async Task<(List<double[]> Coordinates, string GeoJson, double Distance, double Duration, string GoogleMapsUrl)>
    GetRouteAsync(Guid associationId, double centerLat, double centerLng, double radiusKm, double startLat, double startLng)
{
    var centerPoint = new Point(centerLng, centerLat) { SRID = 4326 };

    var events = await _context.GeoEvents
        .Include(e => e.GeoStore)
        .Where(e => e.GeoStore.AssociationId == associationId)
        .Where(e =>
            EF.Property<Point>(e, "Location") != null &&
            EF.Property<Point>(e, "Location")!.Distance(centerPoint) <= radiusKm * 1000)
        .OrderBy(e => e.ObservedAt)
        .ToListAsync();

    if (events.Count == 0)
        return ([], "", 0, 0, "");

    var coordinates = new List<double[]>
    {
        new[] { startLng, startLat }
    };
    coordinates.AddRange(events.Select(e => new[] { e.Longitude, e.Latitude }));

    var httpClient = new HttpClient();
    var orsKey = Environment.GetEnvironmentVariable("ORS_API_KEY");

    if (string.IsNullOrWhiteSpace(orsKey))
        return ([], "", 0, 0, "");

    httpClient.DefaultRequestHeaders.Add("Authorization", orsKey);

    var requestBody = new
    {
        coordinates = coordinates
    };

    var response = await httpClient.PostAsJsonAsync(
        "https://api.openrouteservice.org/v2/directions/foot-walking/geojson", requestBody);

    if (!response.IsSuccessStatusCode)
        return ([], "", 0, 0, "");

    var json = await response.Content.ReadFromJsonAsync<OpenRouteServiceResponse>();
    var feature = json?.features?.FirstOrDefault();

    if (feature == null)
        return ([], "", 0, 0, "");

    var routeCoordinates = feature.geometry?.coordinates ?? new List<double[]>();
    var distance = feature.properties?.summary?.distance ?? 0;
    var duration = feature.properties?.summary?.duration ?? 0;

    var gmapsUrl = BuildGoogleMapsUrl(coordinates);
    var geoJson = JsonConvert.SerializeObject(json);

    return (routeCoordinates, geoJson, distance, duration, gmapsUrl);
}


    private static string BuildGoogleMapsUrl(List<double[]> coords)
    {
        const string baseUrl = "https://www.google.com/maps/dir/";
        var segments = coords.Select(c =>
            string.Create(CultureInfo.InvariantCulture, $"{c[1]},{c[0]}")); // Lat,Lng

        return baseUrl + string.Join("/", segments);
    }

    
    public async Task<GeoStore?> GetGeoStoreByAssociationAsync(Guid associationId)
    {
        return await _context.GeoStores.FirstOrDefaultAsync(x => x.AssociationId == associationId);
    }

    public async Task<GeoStore> CreateGeoStoreAsync(Guid associationId)
    {
        var store = new GeoStore { AssociationId = associationId };
        _context.GeoStores.Add(store);
        await _context.SaveChangesAsync();
        return store;
    }
}