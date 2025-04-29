namespace Maraudr.Stock.Application.UseCases;

public interface ICreateMultipleItemsHandler
{
    Task<IEnumerable<Guid>> HandleAsync(IEnumerable<CreateItemCommand> commands);
}

public class CreateMultipleItems(IStockRepository repository) : ICreateMultipleItemsHandler
{
    public async Task<IEnumerable<Guid>> HandleAsync(IEnumerable<CreateItemCommand> commands)
    {
        var stockItems = commands
            .Select(command => new StockItem(command.Name, command.Description, command.ItemType))
            .ToList();

        await repository.CreateStockItems(stockItems);

        return stockItems.Select(item => item.Id);
    }
}

