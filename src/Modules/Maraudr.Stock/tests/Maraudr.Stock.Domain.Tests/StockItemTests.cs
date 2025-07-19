using FluentAssertions;
using Maraudr.Stock.Domain.Entities;
using Maraudr.Stock.Domain.Enums;
using Maraudr.Stock.Domain.Exceptions;

namespace Maraudr.Stock.Domain.Tests;

public class StockItemTests
{
    
    [Fact]
    public void Constructor_WithAssociationId_ShouldCreateStock()
    {
        // Arrange
        var associationId = Guid.NewGuid();

        // Act
        var stock = new Entities.Stock(associationId);

        // Assert
        stock.Id.Should().NotBeEmpty();
        stock.AssociationId.Should().Be(associationId);
        stock.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddItemToStock()
    {
        // Arrange
        var stock = new Entities.Stock(Guid.NewGuid());
        var item = new StockItem("Test Item");

        // Act
        stock.AddItem(item);

        // Assert
        stock.Items.Should().Contain(item);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemFromStock()
    {
        // Arrange
        var stock = new Entities.Stock(Guid.NewGuid());
        var item = new StockItem("Test Item");
        stock.AddItem(item);

        // Act
        stock.RemoveItem(item);

        // Assert
        stock.Items.Should().NotContain(item);
    }
    
    
    [Fact]
    public void Constructor_WithValidName_ShouldCreateStockItem()
    {
        // Arrange
        var name = "Test Item";
        var description = "Test Description";

        // Act
        var item = new StockItem(name, description);

        // Assert
        item.Id.Should().NotBeEmpty();
        item.Name.Should().Be(name);
        item.Description.Should().Be(description);
        item.Quantity.Should().Be(1);
        item.EntryDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowInvalidItemNameException()
    {
        // Act & Assert
        Assert.Throws<InvalidItemNameException>(() => new StockItem(null));
    }

    [Fact]
    public void RemoveAnItem_WithSufficientQuantity_ShouldReduceQuantity()
    {
        // Arrange
        var item = new StockItem("Test Item");
        item.Quantity = 10;

        // Act
        item.RemoveAnItem(3);

        // Assert
        item.Quantity.Should().Be(7);
    }

    [Fact]
    public void RemoveAnItem_WithInsufficientQuantity_ShouldSetQuantityToZero()
    {
        // Arrange
        var item = new StockItem("Test Item");
        item.Quantity = 5;

        // Act
        item.RemoveAnItem(10);

        // Assert
        item.Quantity.Should().Be(0);
    }
}

