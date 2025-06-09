namespace Maraudr.Stock.Application.UseCases;

public record CreateItemCommand(
    Guid StockId,
    string Name,
    string Description,
    string BarCode,
    Category ItemType
);