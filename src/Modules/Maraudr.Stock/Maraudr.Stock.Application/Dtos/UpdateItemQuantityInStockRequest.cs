namespace Maraudr.Stock.Application.Dtos;

public record UpdateItemQuantityInStockRequest(Guid AssociationId, int? Quantity);