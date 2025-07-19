namespace Maraudr.Stock.Application.UseCases;

public interface IReduceQuantityFromItemHandler
{
    Task HandleAsync(string barcode, Guid associationId, int quantity = 1);
}

public class ReduceQuantityFromItemHandler(IStockRepository repository) : IReduceQuantityFromItemHandler
{
    public async Task HandleAsync(string barcode, Guid associationId, int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentException("La quantité à retirer doit être supérieure à zéro");

        var stock = await repository.GetStockByAssociationIdAsync(associationId);
        if (stock is null)
            throw new ArgumentException("Stock introuvable pour l'association");

        var item = await repository.GetStockItemByBarCodeAndStockIdAsync(barcode, stock.Id);
        if (item is null)
            throw new ArgumentException("Item non trouvé dans le stock");

        if (item.Quantity - quantity <= 0)
        {
            await repository.DeleteStockItemAsync(item.Id);
        }
        else
        {
            await repository.ReduceQuantityFromStockAsync(item.Id, quantity);
        }
    }
}