namespace Maraudr.Stock.Domain.Interfaces;

public interface IStockRepository
{
    Task<StockItem?> GetStockItemByIdAsync(Guid id);
    Task CreateStockItemAsync(StockItem item);
    Task DeleteStockItemAsync(Guid id);
    Task<IEnumerable<StockItem?>> GetStockItemByTypeAsync(Category type);
    Task<StockItem?> GetStockItemByBarCodeAsync(string code);
    Task RemoveQuantityFromStock(Guid id, int quantity);
    Task AddQuantityToStock(Guid id, int newQuantity);



}
