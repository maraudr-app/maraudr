namespace Maraudr.Stock.Domain.Interfaces;

public interface IStockRepository
{
    Task<StockItem?> GetStockItemByIdAsync(Guid id);
    Task CreateStockItemAsync(StockItem item);
    Task DeleteStockItemAsync(Guid id);
    Task<IEnumerable<StockItem?>> GetStockItemByTypeAsync(Category type);
    Task<StockItem?> GetStockItemByBarCodeAsync(string code);
    Task<StockItem?> GetStockItemByBarCodeAndStockIdAsync(string code, Guid stockId);
    Task RemoveQuantityFromStock(Guid id, int quantity);
    Task AddQuantityToStock(Guid id, int newQuantity);
    Task ReduceQuantityFromStockAsync(Guid itemId, int quantity);
    Task CreateStockAsync(Entities.Stock stock);
    Task<IEnumerable<StockItem>> GetItemsByStockIdAsync(Guid stockId);
    Task<StockItem?> GetStockItemByBarCodeAsync(string code, Guid stockId);

    Task DeleteItemFromStockAsync(Guid itemId, Guid stockId);
    Task<Entities.Stock?> GetStockByAssociationIdAsync(Guid associationId);
    Task<Entities.Stock?> GetStockByIdAsync(Guid stockId);
    Task<Guid?> GetStockIdByAssociationIdAsync(Guid associationId);
}
