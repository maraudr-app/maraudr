namespace Maraudr.Stock.Domain.Interfaces;

public interface IStockRepository
{
    Task<StockItem?> GetStockItemByIdAsync(Guid id);
    Task CreateStockItemAsync(StockItem item);
    Task DeleteStockItemAsync(Guid id);
    Task<IEnumerable<StockItem>> GetStockItemByTypeAsync(Category type);
    Task CreateStockItems(IEnumerable<StockItem> items);
}
