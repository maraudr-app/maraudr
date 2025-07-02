namespace Maraudr.MCP.Domain.Interfaces;

public interface IGeoRepository
{
    public Task<IEnumerable<GeoDataDto>> GetAllTodayGeolocalisationPoints(Guid associationId);
}