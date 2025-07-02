namespace Maraudr.MCP.Domain.Interfaces;
public record StockItemDto(      Guid Id ,
    Guid StockId,
    string Name ,
    string? Description ,
    string? BarCode ,
    Category Category ,
    DateTime EntryDate ,
    int Quantity
);

public enum Category
{
    Unknown,
    Food,
    Liquid,
    Medical,
    Clothes,
}

public interface IStockRepository
{
    
    Task<StockItemDto?> GetStockItemByBarCodeAsync(string code,Guid associationId);
    Task<IEnumerable<StockItemDto?>> GetStockItemByTypeAsync(Category type,Guid associationId);
    
    Task<IEnumerable<StockItemDto>> GetStockItemsAsync(Guid associationId);


}