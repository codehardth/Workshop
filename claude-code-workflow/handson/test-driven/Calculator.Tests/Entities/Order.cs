namespace Calculator.Tests.Entities;

public enum OrderState
{
    Pending,
    Submitted,
}

public class Item
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}

public class Order
{
    public List<Item> Items { get; set; } = [];

    public OrderState State { get; set; }

    public void AddItem(Item item)
    {
        if (this.State != OrderState.Pending)
        {
            return;
        }

        if (this.Items.Count >= 5)
        {
            return;
        }

        this.Items.Add(item);
    }

    public void Submit()
    {
        this.State = OrderState.Submitted;
    }
}