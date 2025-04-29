namespace Maraudr.Stock.Infrastructure.Repository;

public class StockRepository(StockContext context) : IStockRepository
{
    private readonly StockContext _context = context;

    public async Task CreateStockItemAsync(StockItem item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStockItemAsync(Guid id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item is not null)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<StockItem?> GetStockItemByIdAsync(Guid id)
    {
        var item = await _context.Items.FindAsync(id);
        return item is null ? null : item;
    }

    public async Task<IEnumerable<StockItem>> GetStockItemByTypeAsync(Category type)
    {
        var item = await _context.Items.Where(x => x.Category == type).ToListAsync();
        return item;
    }
    
    public async Task CreateStockItems(IEnumerable<StockItem> items)
    {
        await _context.Items.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

}
