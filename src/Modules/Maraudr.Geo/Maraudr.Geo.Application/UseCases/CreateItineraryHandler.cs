using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Application.UseCases;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            request.AssociationId,
            request.EventId,
            request.StartLat,
            request.StartLng,
            request.CenterLat,
            request.CenterLng,
            request.RadiusKm,
            geoJson,
            gmapsUrl,
            distance / 1000,
            duration / 60
        );

        await itineraryRepo.AddAsync(itinerary);
        return itinerary;
    }
}
