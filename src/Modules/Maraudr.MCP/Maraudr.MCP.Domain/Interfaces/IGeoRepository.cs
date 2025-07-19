namespace Maraudr.MCP.Domain.Interfaces;

public record GeoDataDto(
   Guid Id,
   Guid GeoStoreId,
   double Latitude,
   double Longitude,
   string Notes,
   DateTime ObservedAt,
   string? Address
);

public interface IGeoRepository
{
   public Task<IEnumerable<GeoDataDto>> GetAllInterestPoints(Guid associationId,int days,string jwt);
}