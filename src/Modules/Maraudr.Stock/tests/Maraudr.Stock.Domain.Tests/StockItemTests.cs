using Maraudr.Stock.Domain.Entities;
using Maraudr.Stock.Domain.Enums;
using Maraudr.Stock.Domain.Exceptions;

namespace Maraudr.Stock.Domain.Tests;

public class StockItemTests
{
    [Fact]
    public void ItemGuid_ShouldNotBeNull_WhenObjectIsInstatianted()
    {
        var item = new StockItem("Sandwich");

        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact]
    public void ItemGuid_ShouldBeDifferent_WhenMutlipleObjectAreInstatianted()
    {
        var sandwich = new StockItem("Sandwich");
        var water = new StockItem("Water");

        Assert.NotEqual(water.Id, sandwich.Id);
    }

    [Fact]
    public void ItemName_ShouldNotBeNull_WhenSetTroughtConstructor()
    {
        var item = new StockItem("Sandwich");

        Assert.Equal("Sandwich", item.Name);
    }

    [Fact]
    public void ItemName_ShouldShouldThrowException_WhenSetTroughtConstructor_IsNull()
    {
        Assert.Throws<InvalidItemNameException>(() => new StockItem(null!));
    }

    [Fact]
    public void Description_ShouldNotBeNull_WhenSet()
    {
        var item = new StockItem("Water", "A bottle of water");

        Assert.Equal("Water", item.Name);
        Assert.Equal("A bottle of water", item.Description);
    }

    [Fact]
    public void Description_ShouldBeNull_WhenNotSet()
    {
        var item = new StockItem();
        
        Assert.Null(item.Description);
    }

    [Fact]
    public void ItemType_ShouldBeUknown_ByDefault()
    {
        var item = new StockItem("Water");
        
        Assert.Equal(Category.Unknown, item.Category);
    }

    [Fact]
    public void ItemType_ShouldBeValid_WhenSetByMethod()
    {
        var item = new StockItem("Cookies", "", Category.Food);

        Assert.Equal(Category.Food, item.Category);
    }
}