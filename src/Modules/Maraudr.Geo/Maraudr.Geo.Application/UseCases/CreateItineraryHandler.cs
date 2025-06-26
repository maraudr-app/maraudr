using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface ICreateItineraryHandler
{
    Task<Itinerary?> HandleAsync(CreateItineraryRequest request);
}

public class CreateItineraryHandler(
    IGeoRepository geoRepo,
    IItineraryRepository itineraryRepo
) : ICreateItineraryHandler
{
    public async Task<Itinerary?> HandleAsync(CreateItineraryRequest request)
    {
        var (coords, geoJson, distance, duration, gmapsUrl) =
            await geoRepo.GetRouteAsync(
                request.AssociationId,
                request.EventId,
                request.CenterLat,
                request.CenterLng,
                request.RadiusKm,
                request.StartLat,
                request.StartLng
            );

        if (coords.Count == 0)
            return null;

        var itinerary = new Itinerary(
            request.EventId,
            request.AssociationId,
            distance / 1000,
            duration / 60,
            geoJson,
            gmapsUrl,
            request.StartLat,
            request.StartLng,
            request.CenterLat,
            request.CenterLng,
            request.RadiusKm
        );


        await itineraryRepo.AddAsync(itinerary);
        return itinerary;
    }
}