namespace Maraudr.Stock.Application.UseCases;


    public interface IUpdateQuantityFromItemHandler
    {
        Task HandleAsync(Guid id, Guid associationId, int quantity = 1);
    }
    public class UpdateQuantityFromItemHandler(IStockRepository repository) : IUpdateQuantityFromItemHandler
    {
        public async Task HandleAsync(Guid id, Guid associationId, int quantity = 1)
        {
           

            var stock = await repository.GetStockByAssociationIdAsync(associationId);
            if (stock is null)
                throw new ArgumentException("Stock introuvable pour l'association");

            var item = await repository.GetStockItemByIdAsync(id);
            if (item is null)
                throw new ArgumentException("Item non trouvé dans le stock");

            if (item.Quantity + quantity <= 0)
            {
                await repository.DeleteStockItemAsync(item.Id);
            }
            else
            {
                await repository.AddQuantityToStock(item.Id, quantity);
            }
        }
    }
