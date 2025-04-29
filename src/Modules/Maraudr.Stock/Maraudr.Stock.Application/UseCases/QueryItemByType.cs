namespace Maraudr.Stock.Application.UseCases;

public interface IQueryItemByType
{
    Task<IEnumerable<StockItemQuery>> HandleAsync(Category type);
}

public class QueryItemByType(IStockRepository repository) : IQueryItemByType
{
    public async Task<IEnumerable<StockItemQuery>> HandleAsync(Category type)
    {
        var items = await repository.GetStockItemByTypeAsync(type);

        return items.Select(item => new StockItemQuery(item.Id, item.Name, item.Description, item.Category, item.EntryDate));
    }
}
