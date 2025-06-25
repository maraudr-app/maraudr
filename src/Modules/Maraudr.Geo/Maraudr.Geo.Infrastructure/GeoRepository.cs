using System.Globalization;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using Maraudr.Geo.Infrastructure.GeoData;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
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

    public async Task<(string?, string)> GetRouteAsync(Guid associationId, double lat, double lng, double radiusKm)
    {
        var radiusMeters = radiusKm * 1000;
        var centerPoint = new Point(lng, lat) { SRID = 4326 };

        // 1. Récupérer les GeoData proches
        var events = await _context.GeoEvents
            .Where(e => e.GeoStore!.AssociationId == associationId)
            .Select(e => new
            {
                e.Latitude,
                e.Longitude,
                e.ObservedAt
            })
            .ToListAsync();

        // 2. Filtrer en mémoire avec distance Haversine (pas ST_DWithin)
        var close = events
            .Where(e => GeoUtils.GetDistanceInKm(lat, lng, e.Latitude, e.Longitude) <= radiusKm)
            .OrderBy(e => e.ObservedAt)
            .ToList();

        if (close.Count == 0)
            return (null, "");

        // 3. Créer une LineString
        var coordinates = close.Select(p =>
            new Coordinate(p.Longitude, p.Latitude)).ToArray();

        var line = new LineString(coordinates) { SRID = 4326 };

        var geoJsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
        var geoJson = geoJsonWriter.Write(line);

        // 4. URL Google Maps
        var gmapsUrl = "https://www.google.com/maps/dir/" +
                       string.Join("/", close.Select(p =>
                           $"{p.Latitude.ToString(CultureInfo.InvariantCulture)},{p.Longitude.ToString(CultureInfo.InvariantCulture)}"));

        return (geoJson, gmapsUrl);
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