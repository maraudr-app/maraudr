using Maraudr.Stock.Application.Dtos;

namespace Maraudr.Stock.Application.UseCases;

public interface IGetItemsOfAssociationHandler
{
    Task<IEnumerable<StockItem>> HandleAsync(Guid associationId, ItemFilter filter);
}

public class GetItemsOfAssociationHandler(IStockRepository repository) : IGetItemsOfAssociationHandler
{
    public async Task<IEnumerable<StockItem>> HandleAsync(Guid associationId, ItemFilter filter)
    {
        var stock = await repository.GetStockByAssociationIdAsync(associationId);
        if (stock == null)
            return [];

        var query = stock.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Category) &&
            Enum.TryParse<Category>(filter.Category, ignoreCase: true, out var parsedCategory))
        {
            query = query.Where(item => item.Category == parsedCategory);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(item => item.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }
}
