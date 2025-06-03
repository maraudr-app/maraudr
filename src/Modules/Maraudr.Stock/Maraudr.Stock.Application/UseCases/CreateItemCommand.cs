namespace Maraudr.Stock.Application.UseCases;

public record CreateItemCommand(string Name, string Description, string BarCode, Category ItemType);