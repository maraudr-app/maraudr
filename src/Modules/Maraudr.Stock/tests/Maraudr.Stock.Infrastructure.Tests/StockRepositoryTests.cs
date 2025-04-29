using Maraudr.Stock.Domain.Entities;
using Maraudr.Stock.Domain.Enums;
using Maraudr.Stock.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Stock.Infrastructure.Tests
{
    public class StockRepositoryTests
    {
        private readonly DbContextOptions<StockContext> _options = new DbContextOptionsBuilder<StockContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        [Fact]
        public async Task CreateStockItem_Should_Add_Item_To_Database()
        {
            var context = new StockContext(_options);
            var repository = new StockRepository(context);
            var newItem = new StockItem("Water", "Bottle of water", Category.Unknown);

            await repository.CreateStockItemAsync(newItem);

            var item = await context.Items.FindAsync(newItem.Id);
            Assert.NotNull(item);
            Assert.Equal("Water", item.Name);
            Assert.Equal("Bottle of water", item.Description);
            Assert.Equal(Category.Unknown, item.Category);
        }
    }
}