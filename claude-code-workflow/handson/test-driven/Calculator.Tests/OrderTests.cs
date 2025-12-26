namespace Calculator.Tests;

using Entities;

public class OrderTests
{
    [Fact]
    public void Order_WhenAddItem_WhilePending_ShouldAddItemToOrder()
    {
        // Arrange
        var order = new Order();
        var item = new Item { Id = 1, Name = "Test Item", Price = 10 };

        // Act
        order.AddItem(item);

        // Assert
        Assert.Contains(item, order.Items);
    }

    [Fact]
    public void Order_WhenAddItem_WhileSubmitted_ShouldNotAddItem()
    {
        // Arrange
        var order = new Order();
        var item = new Item { Id = 1, Name = "Test Item", Price = 10 };

        // Act
        order.Submit();
        order.AddItem(item);

        // Assert
        Assert.DoesNotContain(item, order.Items);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Order_WhenAddItem_MustNotExceedFiveItems(int itemCount)
    {
        // Arrange
        var order = new Order();

        // Act
        for (var i = 0; i < itemCount; i++)
        {
            order.AddItem(new Item
            {
                Id = i,
                Name = $"Item {i}",
                Price = 10,
            });
        }

        // Assert
        Assert.True(order.Items.Count <= 5);
    }
}