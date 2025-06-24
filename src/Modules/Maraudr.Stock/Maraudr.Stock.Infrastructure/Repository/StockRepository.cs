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
        return item;
    }

    public async Task<IEnumerable<StockItem?>> GetStockItemByTypeAsync(Category type)
    {
        var item = await _context.Items.Where(x => x.Category == type).ToListAsync();
        return item;
    }
    

    public async Task<StockItem?> GetStockItemByBarCodeAsync(string code)
    {
        var items = await _context.Items.Where(x => x.BarCode != null && x.BarCode.Equals(code)).ToListAsync();
        try
        {
            return items.First();
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public async Task<StockItem?> GetStockItemByBarCodeAndStockIdAsync(string code, Guid stockId)
    {
        return await _context.Items
            .FirstOrDefaultAsync(x => x.BarCode != null && x.BarCode.Equals(code) && x.StockId == stockId);
    }
    
    public async Task AddQuantityToStock(Guid id, int newQuantity)
    {
        var item = await _context.Items.FindAsync(id);
        if (item != null)
        {
            item.Quantity += newQuantity;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task ReduceQuantityFromStockAsync(Guid itemId, int quantity)
    {
        var item = await _context.Items.FindAsync(itemId);
        if (item != null)
        {
            item.Quantity -= quantity;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task RemoveQuantityFromStock(Guid id, int newQuantity)
    {
        var item = await _context.Items.FindAsync(id);
        if (item != null)
        {
            item.RemoveAnItem(newQuantity);
            if (item.Quantity == 0)
            {
                _context.Items.Remove(item);
                
            }
            else
            {
                _context.Items.Update(item);
                
            }
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task CreateStockAsync(Domain.Entities.Stock stock)
    {
        await _context.Stocks.AddAsync(stock);
        await _context.SaveChangesAsync();
    }
    
    public async Task<StockItem?> GetStockItemByBarCodeAsync(string code, Guid stockId)
    {
        return await _context.Items
            .Where(x => x.BarCode == code && x.StockId == stockId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<StockItem>> GetItemsByStockIdAsync(Guid stockId)
    {
        return await _context.Items
            .Where(item => item.StockId == stockId)
            .ToListAsync();
    }
    
    public async Task DeleteItemFromStockAsync(Guid itemId, Guid stockId)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.StockId == stockId);
        if (item is not null)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<Domain.Entities.Stock?> GetStockByAssociationIdAsync(Guid associationId)
    {
        return await _context.Stocks
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.AssociationId == associationId);
    }
    
    public async Task<Domain.Entities.Stock?> GetStockByIdAsync(Guid stockId)
    {
        return await _context.Stocks.FindAsync(stockId);
    }
    
    public async Task<Guid?> GetStockIdByAssociationIdAsync(Guid associationId)
    {
        var stock = await _context.Stocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AssociationId == associationId);

        return stock?.Id;
    }
}
